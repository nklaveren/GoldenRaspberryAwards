using GoldenRaspberryAwards.Api.Modules.Awards.Application.DTOs;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.Api.Modules.Awards.Presentation;

public static class AwardsEndpoints
{
    public static void MapAwardsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/awards")
            .WithTags("Awards");

        group.MapGet("/intervals", GetAwardIntervals)
            .WithName("GetAwardIntervals")
            .WithSummary("Retorna os produtores com maior e menor intervalo entre vitórias consecutivas")
            .Produces<AwardIntervalsResult>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetAwardIntervals(AwardsDbContext db)
    {
        var minInterval = await db.ProducerIntervals.MinAsync(x => (int?)x.Interval);
        var maxInterval = await db.ProducerIntervals.MaxAsync(x => (int?)x.Interval);

        if (minInterval is null || maxInterval is null)
            return Results.Ok(new AwardIntervalsResult([], []));

        var minProducers = await db.ProducerIntervals
            .Where(x => x.Interval == minInterval)
            .Select(x => ProducerIntervalDto.ToDto(x))
            .ToListAsync();

        var maxProducers = await db.ProducerIntervals
            .Where(x => x.Interval == maxInterval)
            .Select(x => ProducerIntervalDto.ToDto(x))
            .ToListAsync();

        return Results.Ok(new AwardIntervalsResult(minProducers, maxProducers));
    }
}
