namespace GoldenRaspberryAwards.Api.Modules.Awards.Domain.ValueObjects;

public record AwardInterval
{
    public string ProducerName { get; init; }
    public int PreviousWin { get; init; }
    public int FollowingWin { get; init; }
    public int Interval => FollowingWin - PreviousWin;

    private AwardInterval(string producerName, int previousWin, int followingWin)
    {
        ProducerName = producerName;
        PreviousWin = previousWin;
        FollowingWin = followingWin;
    }

    public static Result<AwardInterval> Create(string producerName, int previousWin, int followingWin)
    {
        if (string.IsNullOrWhiteSpace(producerName))
            return Result.Failure<AwardInterval>("Producer name cannot be empty");

        if (followingWin <= previousWin)
            return Result.Failure<AwardInterval>("Following win must be after previous win");

        return Result.Success(new AwardInterval(producerName, previousWin, followingWin));
    }
}
