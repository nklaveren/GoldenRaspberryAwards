using GoldenRaspberryAwards.Api.Modules.Awards.Domain.Events;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwards.Api.Modules.Awards.Application.Handlers;

public class ProducerAwardEventProcessor(AwardsDbContext dbContext)
{
    public async Task HandleAsync(ProducerAwardedEvent evt)
    {
        var existingWin = await dbContext.ProducerWins
            .FirstOrDefaultAsync(w => w.ProducerName == evt.ProducerName && w.Year == evt.Year);

        if (existingWin == null)
        {
            dbContext.ProducerWins.Add(new ProducerWinEntity
            {
                ProducerName = evt.ProducerName,
                Year = evt.Year
            });
            await dbContext.SaveChangesAsync();
        }

        await RecalculateIntervalsAsync(evt.ProducerName);
    }

    private async Task RecalculateIntervalsAsync(string producerName)
    {
        var wins = await dbContext.ProducerWins
            .Where(w => w.ProducerName == producerName)
            .OrderBy(w => w.Year)
            .ToListAsync();

        if (wins.Count < 2) return;

        var existingIntervals = await dbContext.ProducerIntervals
            .Where(i => i.ProducerName == producerName)
            .ToListAsync();

        dbContext.ProducerIntervals.RemoveRange(existingIntervals);

        for (int i = 1; i < wins.Count; i++)
        {
            dbContext.ProducerIntervals.Add(new ProducerIntervalEntity
            {
                ProducerName = producerName,
                PreviousWin = wins[i - 1].Year,
                FollowingWin = wins[i].Year,
                Interval = wins[i].Year - wins[i - 1].Year
            });
        }

        await dbContext.SaveChangesAsync();
    }
}
