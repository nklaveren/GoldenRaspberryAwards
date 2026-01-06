namespace GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure.Entities;

public class ProducerIntervalEntity
{
    public int Id { get; set; }
    public string ProducerName { get; set; } = string.Empty;
    public int PreviousWin { get; set; }
    public int FollowingWin { get; set; }
    public int Interval { get; set; }
}
