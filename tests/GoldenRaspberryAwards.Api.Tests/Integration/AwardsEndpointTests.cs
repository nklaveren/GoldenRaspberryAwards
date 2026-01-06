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

        var min = Assert.Single(result.Min);
        Assert.Equal("Producer Min", min.Producer);
        Assert.Equal(1, min.Interval);
        Assert.Equal(1981, min.PreviousWin);
        Assert.Equal(1982, min.FollowingWin);

        var max = Assert.Single(result.Max);
        Assert.Equal("Producer Max", max.Producer);
        Assert.Equal(13, max.Interval);
        Assert.Equal(1983, max.PreviousWin);
        Assert.Equal(1996, max.FollowingWin);
    }
}
