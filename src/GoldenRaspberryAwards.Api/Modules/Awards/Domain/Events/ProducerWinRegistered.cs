namespace GoldenRaspberryAwards.Api.Modules.Awards.Domain.Events;

public record ProducerAwardedEvent(
    string ProducerName,
    int Year
) : IDomainEvent;
