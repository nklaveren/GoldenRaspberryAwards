using GoldenRaspberryAwards.Api.Modules.Awards.Domain.Entities;
using GoldenRaspberryAwards.Api.Modules.Awards.Domain.Events;
using GoldenRaspberryAwards.Api.Modules.Awards.Domain.ValueObjects;

namespace GoldenRaspberryAwards.Api.Tests.Domain;

public class ProducerTests
{
    [Fact]
    public void RecordWin_ShouldAddYearToWinYears()
    {
        // Arrange
        var producer = Producer.Create("Allan Carr").Value!;

        // Act
        var result = producer.RecordWin(2000);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(producer.WinYears);
        Assert.Contains(2000, producer.WinYears);
    }

    [Fact]
    public void RecordWin_ShouldEmitDomainEvent()
    {
        // Arrange
        var producer = Producer.Create("Allan Carr").Value!;

        // Act
        producer.RecordWin(2000);

        // Assert
        Assert.Single(producer.DomainEvents);
        var evt = Assert.IsType<ProducerAwardedEvent>(producer.DomainEvents.First());
        Assert.Equal("Allan Carr", evt.ProducerName);
        Assert.Equal(2000, evt.Year);
    }

    [Fact]
    public void RecordWin_DuplicateYear_ShouldNotAddAgain()
    {
        // Arrange
        var producer = Producer.Create("Allan Carr").Value!;
        producer.RecordWin(2000);

        // Act
        var result = producer.RecordWin(2000);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(producer.WinYears);
        Assert.Single(producer.DomainEvents);
    }

    [Fact]
    public void GetConsecutiveIntervals_WithTwoWins_ShouldReturnOneInterval()
    {
        // Arrange
        var producer = Producer.Create("Jerry Weintraub").Value!;
        producer.RecordWin(2000);
        producer.RecordWin(2005);

        // Act
        var intervals = producer.GetConsecutiveIntervals().ToList();

        // Assert
        Assert.Single(intervals);
        Assert.Equal(5, intervals[0].Interval);
        Assert.Equal(2000, intervals[0].PreviousWin);
        Assert.Equal(2005, intervals[0].FollowingWin);
    }

    [Fact]
    public void GetConsecutiveIntervals_WithThreeWins_ShouldReturnTwoIntervals()
    {
        // Arrange
        var producer = Producer.Create("Jerry Weintraub").Value!;
        producer.RecordWin(2000);
        producer.RecordWin(2005);
        producer.RecordWin(2008);

        // Act
        var intervals = producer.GetConsecutiveIntervals().ToList();

        // Assert
        Assert.Equal(2, intervals.Count);
        Assert.Equal(5, intervals[0].Interval);
        Assert.Equal(3, intervals[1].Interval);
    }

    [Fact]
    public void GetConsecutiveIntervals_WithOneWin_ShouldReturnEmpty()
    {
        // Arrange
        var producer = Producer.Create("Steve Shagan").Value!;
        producer.RecordWin(2000);

        // Act
        var intervals = producer.GetConsecutiveIntervals().ToList();

        // Assert
        Assert.Empty(intervals);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldReturnFailure()
    {
        // Act & Assert
        Assert.True(Producer.Create("").IsFailure);
        Assert.True(Producer.Create("   ").IsFailure);
    }

    [Fact]
    public void RecordWin_WithYearBefore1980_ShouldReturnFailure()
    {
        // Arrange
        var producer = Producer.Create("Allan Carr").Value!;

        // Act
        var result = producer.RecordWin(1979);

        // Assert
        Assert.True(result.IsFailure);
    }
}

public class AwardIntervalTests
{
    [Fact]
    public void Interval_ShouldCalculateDifference()
    {
        // Arrange & Act
        var result = AwardInterval.Create("Allan Carr", 2000, 2005);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value!.Interval);
    }

    [Fact]
    public void Create_WithInvalidRange_ShouldReturnFailure()
    {
        // Act & Assert
        Assert.True(AwardInterval.Create("Allan Carr", 2005, 2000).IsFailure);
        Assert.True(AwardInterval.Create("Allan Carr", 2000, 2000).IsFailure);
    }

    [Fact]
    public void Create_WithEmptyProducerName_ShouldReturnFailure()
    {
        // Act & Assert
        Assert.True(AwardInterval.Create("", 2000, 2005).IsFailure);
    }
}
