using System.Text.Json;
using Application.DayPlan;
using Tests.TestUtilities;
using Xunit;
using System;
using Application.DayPlan.DTOs;
using Xunit.Abstractions;

namespace DishCraft.Tests.IntegrationTests;

public class DayPlanControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly TestHelperMethods _testHelperMethods;

    public DayPlanControllerTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _client = factory.CreateClient();
        _testHelperMethods = new TestHelperMethods(_client);
    }

    [Fact]
    public async Task CreateDayPlan_ShouldReturnCreated()
    {
        _testOutputHelper.WriteLine("🌟 Starting CreateDayPlan test...");
        var dayPlanId = await _testHelperMethods.CreateDayPlan("Test Plan");

        _testOutputHelper.WriteLine($"✅ Created DayPlan with ID: {dayPlanId}");

        Assert.NotEqual(Guid.Empty, dayPlanId);
    }

    [Fact]
    public async Task AddRecipeToDayPlan_ShouldWork()
    {
        _testOutputHelper.WriteLine("🌟 Starting AddRecipeToDayPlan test...");

        var recipeId = await _testHelperMethods.CreateRecipe();
        _testOutputHelper.WriteLine($"✅ Created Recipe with ID: {recipeId}");

        var dayPlanId = await _testHelperMethods.CreateDayPlan("Test Plan");
        _testOutputHelper.WriteLine($"✅ Created DayPlan with ID: {dayPlanId}");

        var dayPlanRecipeId = await _testHelperMethods.CreateDayPlanRecipe(dayPlanId, recipeId, 2);
        _testOutputHelper.WriteLine($"✅ Added Recipe to DayPlan. DayPlanRecipe ID: {dayPlanRecipeId}");

        Assert.NotEqual(Guid.Empty, dayPlanRecipeId);
    }

    [Fact]
    public async Task GetDayPlan_ShouldReturnCorrectData()
    {
        _testOutputHelper.WriteLine("🌟 Starting GetDayPlan test...");

        var recipeId = await _testHelperMethods.CreateRecipe();
        _testOutputHelper.WriteLine($"✅ Created Recipe with ID: {recipeId}");

        var dayPlanId = await _testHelperMethods.CreateDayPlan("Test Plan");
        _testOutputHelper.WriteLine($"✅ Created DayPlan with ID: {dayPlanId}");

        await _testHelperMethods.CreateDayPlanRecipe(dayPlanId, recipeId, 2);
        _testOutputHelper.WriteLine("✅ Added Recipe to DayPlan");

        var response = await _client.GetAsync($"/api/dayplan/{dayPlanId}");
        _testOutputHelper.WriteLine($"🌐 GET request to /api/dayplan/{dayPlanId}");

        response.EnsureSuccessStatusCode();
        _testOutputHelper.WriteLine("✅ GET request succeeded");

        var responseContent = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"📨 Response Content: {responseContent}");

        var retrievedDayPlan = JsonSerializer.Deserialize<DayPlanDto>(
            responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(retrievedDayPlan);
        //Assert.Equal("Test Plan", retrievedDayPlan.Name);
        Assert.Single(retrievedDayPlan.DayPlanRecipes);
        Assert.Equal(2, retrievedDayPlan.DayPlanRecipes[0].Servings);
        _testOutputHelper.WriteLine("✅ GetDayPlan test completed successfully");
    }
}
