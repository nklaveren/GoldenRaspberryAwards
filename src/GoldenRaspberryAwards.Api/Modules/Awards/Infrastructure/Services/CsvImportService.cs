using System.Text.RegularExpressions;
using GoldenRaspberryAwards.Api.Modules.Awards.Application.Handlers;
using GoldenRaspberryAwards.Api.Modules.Awards.Domain;
using GoldenRaspberryAwards.Api.Modules.Awards.Domain.Entities;
using GoldenRaspberryAwards.Api.Modules.Awards.Domain.Events;

namespace GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure.Services;

public class CsvImportService(
    ProducerAwardEventProcessor projectionHandler,
    ILogger<CsvImportService> logger)
{
    public async Task ImportFromCsvAsync(string csvPath)
    {
        if (!File.Exists(csvPath))
        {
            logger.LogWarning("CSV file not found: {Path}", csvPath);
            return;
        }

        var lines = await File.ReadAllLinesAsync(csvPath);
        if (lines.Length <= 1)
        {
            logger.LogWarning("CSV file is empty or has only header");
            return;
        }

        var producers = new Dictionary<string, Producer>();
        var moviesCount = 0;

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var movieData = ParseCsvLine(line);
            if (movieData == null) continue;

            moviesCount++;

            if (!movieData.Winner) continue;

            var producerNames = ParseProducers(movieData.Producers);
            foreach (var producerName in producerNames)
            {
                if (!producers.TryGetValue(producerName, out var producer))
                {
                    var createResult = Producer.Create(producerName);
                    if (createResult.IsFailure)
                    {
                        logger.LogWarning("Invalid producer name: {Name}. Error: {Error}", producerName, createResult.Error);
                        continue;
                    }
                    producer = createResult.Value!;
                    producers[producerName] = producer;
                }

                var recordResult = producer.RecordWin(movieData.Year);
                if (recordResult.IsFailure)
                {
                    logger.LogWarning("Failed to record win for producer {Name} in {Year}: {Error}", producerName, movieData.Year, recordResult.Error);
                }
            }
        }

        foreach (var producer in producers.Values)
        {
            foreach (var evt in producer.DomainEvents.OfType<ProducerAwardedEvent>())
            {
                await projectionHandler.HandleAsync(evt);
            }
            producer.ClearDomainEvents();
        }

        logger.LogInformation("Imported {MovieCount} movies and processed {ProducerCount} producers",
            moviesCount,
            producers.Count);
    }

    private record MovieData(int Year, string Title, string Studios, string Producers, bool Winner);

    private MovieData? ParseCsvLine(string line)
    {
        try
        {
            var parts = line.Split(';');
            if (parts.Length < 4) return null;

            return new MovieData(
                Year: int.Parse(parts[0]),
                Title: parts[1],
                Studios: parts[2],
                Producers: parts[3],
                Winner: parts.Length > 4 && parts[4].Equals("yes", StringComparison.OrdinalIgnoreCase)
            );
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to parse CSV line: {Line}", line);
            return null;
        }
    }

    private static IEnumerable<string> ParseProducers(string producers)
    {
        if (string.IsNullOrWhiteSpace(producers))
            return Enumerable.Empty<string>();

        var names = Regex.Split(producers, @",\s*|\s+and\s+", RegexOptions.IgnoreCase);

        return names
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct();
    }
}
