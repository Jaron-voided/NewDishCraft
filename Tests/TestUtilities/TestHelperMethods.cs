using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.DayPlan;
using Application.DayPlanRecipe;
using Application.Ingredients;
using Application.Measurements;
using Application.Recipes;

namespace Tests.TestUtilities
{
    public class TestHelperMethods
    {
        private readonly HttpClient _client;
        private readonly string _testUserId = "746d6986-cbf9-4221-8350-21f7daa42c7b";

        public TestHelperMethods(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestAuth");
        }

        private async Task<string> GetResponseContent(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"📨 Response Status: {response.StatusCode}");
            Console.WriteLine($"📨 Response Content: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Response failed with status: {response.StatusCode}");
                throw new Exception($"❌ Unexpected response status: {response.StatusCode}. Content: {responseContent}");
            }

            if (string.IsNullOrWhiteSpace(responseContent))
                throw new Exception("❌ API response is empty or invalid.");

            return responseContent;
        }

        public async Task<Guid> CreateIngredient(string name, string category)
        {
            Console.WriteLine($"🌟 Creating Ingredient with Name: {name}, Category: {category}");

            var newIngredient = new IngredientDto
            {
                Name = name,
                Category = category,
                MeasuredIn = "Weight",
                PricePerPackage = 10m,
                MeasurementsPerPackage = 1000,
                Calories = 50,
                Carbs = 10,
                Fat = 5,
                Protein = 3,
                AppUserId = _testUserId
            };

            var content = new StringContent(JsonSerializer.Serialize(newIngredient), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/ingredients", content);
            Console.WriteLine($"🌟 CreateIngredient Response: {response.StatusCode}");

            response.EnsureSuccessStatusCode();
            var responseContent = await GetResponseContent(response);
            var createdIngredient = JsonSerializer.Deserialize<IngredientDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"✅ Ingredient created successfully with ID: {createdIngredient?.Id}");
            return createdIngredient?.Id ?? throw new Exception("❌ Failed to create ingredient.");
        }

        public async Task<Guid> CreateMeasurement(Guid recipeId, Guid ingredientId, int amount)
        {
            Console.WriteLine($"🌟 Creating Measurement for Recipe: {recipeId}, Ingredient: {ingredientId}, Amount: {amount}");

            var newMeasurement = new MeasurementDto
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Amount = amount
            };

            var content = new StringContent(JsonSerializer.Serialize(newMeasurement), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/measurements", content);
            Console.WriteLine($"🌟 CreateMeasurement Response: {response.StatusCode}");

            response.EnsureSuccessStatusCode();
            var responseContent = await GetResponseContent(response);
            var createdMeasurement = JsonSerializer.Deserialize<MeasurementDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"✅ Measurement created successfully with ID: {createdMeasurement?.Id}");
            return createdMeasurement?.Id ?? throw new Exception("❌ Failed to create measurement.");
        }

        public async Task<Guid> CreateRecipe()
        {
            Console.WriteLine($"🌟 Creating Recipe...");

            var ingredient1Id = await CreateIngredient("Ingredient 1", "Baking");
            var ingredient2Id = await CreateIngredient("Ingredient 2", "Spice");

            var newRecipe = new RecipeDto
            {
                Name = "Test Recipe",
                RecipeCategory = "Soup",
                ServingsPerRecipe = 10,
                Instructions = new List<string> { "Step 1: Chop veggies", "Step 2: Boil water", "Step 3: Add ingredients" },
                AppUserId = _testUserId,
                TotalPrice = 100m,
                PricePerServing = 10m,
                CaloriesPerRecipe = 200,
                CarbsPerRecipe = 40,
                FatPerRecipe = 10,
                ProteinPerRecipe = 15
            };

            var content = new StringContent(JsonSerializer.Serialize(newRecipe), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/recipes", content);
            Console.WriteLine($"🌟 CreateRecipe Response: {response.StatusCode}");

            response.EnsureSuccessStatusCode();
            var responseContent = await GetResponseContent(response);
            var createdRecipe = JsonSerializer.Deserialize<RecipeDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"✅ Recipe created successfully with ID: {createdRecipe?.Id}");

            await CreateMeasurement(createdRecipe.Id, ingredient1Id, 200);
            await CreateMeasurement(createdRecipe.Id, ingredient2Id, 150);

            return createdRecipe?.Id ?? throw new Exception("❌ Failed to create recipe.");
        }

        public async Task<Guid> CreateDayPlan(string name)
        {
            Console.WriteLine($"🌟 Creating DayPlan with Name: {name}");

            var newDayPlan = new DayPlanDto
            {
                Name = name,
                AppUserId = _testUserId
            };

            var content = new StringContent(JsonSerializer.Serialize(newDayPlan), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/dayplans", content);
            Console.WriteLine($"🌟 CreateDayPlan Response: {response.StatusCode}");

            response.EnsureSuccessStatusCode();
            var responseContent = await GetResponseContent(response);
            var createdDayPlan = JsonSerializer.Deserialize<DayPlanDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"✅ DayPlan created successfully with ID: {createdDayPlan?.Id}");
            return createdDayPlan?.Id ?? throw new Exception("❌ Failed to create DayPlan.");
        }

        public async Task<Guid> CreateDayPlanRecipe(Guid dayPlanId, Guid recipeId, int servings)
        {
            Console.WriteLine($"🌟 Adding Recipe to DayPlan. DayPlanID: {dayPlanId}, RecipeID: {recipeId}, Servings: {servings}");

            var newDayPlanRecipe = new DayPlanRecipeDto
            {
                RecipeId = recipeId,
                Servings = servings
            };

            var content = new StringContent(JsonSerializer.Serialize(newDayPlanRecipe), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"/api/dayplans/{dayPlanId}", content);
            Console.WriteLine($"🌟 CreateDayPlanRecipe Response: {response.StatusCode}");

            response.EnsureSuccessStatusCode();
            var responseContent = await GetResponseContent(response);
            var createdDayPlanRecipe = JsonSerializer.Deserialize<DayPlanRecipeDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine($"✅ DayPlanRecipe created successfully with ID: {createdDayPlanRecipe?.Id}");
            return createdDayPlanRecipe?.Id ?? throw new Exception("❌ Failed to create DayPlanRecipe.");
        }
    }
}
