using System.Data.Common;
using System.Net.Http.Json;
using GoldenRaspberryAwards.Api.Modules.Awards.Application.DTOs;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenRaspberryAwards.Api.Tests.Integration;

public class AwardsEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AwardsEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AwardsDbContext>();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetIntervals_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/awards/intervals");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetIntervals_ShouldReturnCorrectFormat()
    {
        // Act
        var result = await _client.GetFromJsonAsync<AwardIntervalsResult>("/api/awards/intervals");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Min);
        Assert.NotNull(result.Max);
    }

    [Fact]
    public async Task GetIntervals_ShouldReturnExpectedDeterministicValues()
    {
        // Act
        var result = await _client.GetFromJsonAsync<AwardIntervalsResult>("/api/awards/intervals");

        // Assert
        Assert.NotNull(result);

        // Min: dois produtores com intervalo 1
        Assert.Equal(2, result.Min.Count());

        var minProducer3 = result.Min.First(x => x.Producer == "Producer 3");
        Assert.Equal(1, minProducer3.Interval);
        Assert.Equal(2018, minProducer3.PreviousWin);
        Assert.Equal(2019, minProducer3.FollowingWin);

        var minProducer4 = result.Min.First(x => x.Producer == "Producer 4");
        Assert.Equal(1, minProducer4.Interval);
        Assert.Equal(2020, minProducer4.PreviousWin);
        Assert.Equal(2021, minProducer4.FollowingWin);

        // Max: dois produtores com intervalo 99
        Assert.Equal(2, result.Max.Count());

        var maxProducer1 = result.Max.First(x => x.Producer == "Producer 1");
        Assert.Equal(99, maxProducer1.Interval);
        Assert.Equal(1980, maxProducer1.PreviousWin);
        Assert.Equal(2079, maxProducer1.FollowingWin);

        var maxProducer2 = result.Max.First(x => x.Producer == "Producer 2");
        Assert.Equal(99, maxProducer2.Interval);
        Assert.Equal(1980, maxProducer2.PreviousWin);
        Assert.Equal(2079, maxProducer2.FollowingWin);
    }
}
