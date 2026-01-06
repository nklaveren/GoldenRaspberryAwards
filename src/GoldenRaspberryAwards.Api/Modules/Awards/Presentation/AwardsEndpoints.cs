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
        var intervals = await db.ProducerIntervals.ToListAsync();

        if (intervals.Count == 0)
            return Results.Ok(new AwardIntervalsResult([], []));

        var minInterval = intervals.Min(x => x.Interval);
        var maxInterval = intervals.Max(x => x.Interval);

        return Results.Ok(new AwardIntervalsResult(
            Min: intervals.Where(x => x.Interval == minInterval).Select(ProducerIntervalDto.ToDto),
            Max: intervals.Where(x => x.Interval == maxInterval).Select(ProducerIntervalDto.ToDto)
        ));
    }
}
