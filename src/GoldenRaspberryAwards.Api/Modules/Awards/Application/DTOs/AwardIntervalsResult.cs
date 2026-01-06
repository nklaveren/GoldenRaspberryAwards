using System.Text.Json.Serialization;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure.Entities;

namespace GoldenRaspberryAwards.Api.Modules.Awards.Application.DTOs;

public record ProducerIntervalDto(
    [property: JsonPropertyName("producer")] string Producer,
    [property: JsonPropertyName("interval")] int Interval,
    [property: JsonPropertyName("previousWin")] int PreviousWin,
    [property: JsonPropertyName("followingWin")] int FollowingWin
)
{
    public static ProducerIntervalDto ToDto(ProducerIntervalEntity entity) =>
        new(entity.ProducerName, entity.Interval, entity.PreviousWin, entity.FollowingWin);
}

public record AwardIntervalsResult(
    [property: JsonPropertyName("min")] IEnumerable<ProducerIntervalDto> Min,
    [property: JsonPropertyName("max")] IEnumerable<ProducerIntervalDto> Max
);