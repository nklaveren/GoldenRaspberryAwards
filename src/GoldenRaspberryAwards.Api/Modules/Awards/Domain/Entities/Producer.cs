using GoldenRaspberryAwards.Api.Modules.Awards.Domain.Events;
using GoldenRaspberryAwards.Api.Modules.Awards.Domain.ValueObjects;

namespace GoldenRaspberryAwards.Api.Modules.Awards.Domain.Entities;

public class Producer
{
    public string Name { get; private set; }
    private readonly SortedSet<int> _winYears = [];
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<int> WinYears => _winYears;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Producer(string name) => Name = name;

    public static Result<Producer> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Producer>("Producer name cannot be empty");

        return Result.Success(new Producer(name.Trim()));
    }

    public Result<bool> RecordWin(int year)
    {
        const int minAwardYear = 1980;
        if (year < minAwardYear)
            return Result.Failure<bool>($"Award year must be {minAwardYear} or later");

        if (_winYears.Add(year))
        {
            _domainEvents.Add(new ProducerAwardedEvent(Name, year));
            return Result.Success(true);
        }

        return Result.Success(false);
    }

    public IEnumerable<AwardInterval> GetConsecutiveIntervals()
    {
        var years = _winYears.ToList();

        for (int i = 1; i < years.Count; i++)
        {
            var result = AwardInterval.Create(Name, years[i - 1], years[i]);
            if (result.IsSuccess)
                yield return result.Value!;
        }
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
