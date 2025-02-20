using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Ingredients;
using Application.Measurements;
using Application.Recipes;
using Tests.TestUtilities;

namespace DishCraft.Tests.IntegrationTests
{
    public class RecipeControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly string _testUserId = "746d6986-cbf9-4221-8350-21f7daa42c7b";

        public RecipeControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            // ✅ Set authentication header to simulate an authenticated user
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestAuth");
        }
        
        private async Task<Guid> CreateIngredient(string name, string category)
        {
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
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdIngredient = JsonSerializer.Deserialize<IngredientDto>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (createdIngredient == null || createdIngredient.Id == Guid.Empty)
                throw new Exception($"❌ Failed to retrieve a valid Ingredient ID from response: {responseContent}");

            return createdIngredient.Id;
        }
        private async Task CreateMeasurement(Guid recipeId, Guid ingredientId, int amount)
        {
            var newMeasurement = new MeasurementDto
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Amount = amount
            };

            var content = new StringContent(JsonSerializer.Serialize(newMeasurement), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/measurements", content);
            response.EnsureSuccessStatusCode();
        }

        // Helper method: Create Recipe and return its ID
        private async Task<Guid> CreateRecipe()
        {
            // ✅ Create 2 Ingredients first
            var ingredient1Id = await CreateIngredient("Ingredient 1", "Baking");
            var ingredient2Id = await CreateIngredient("Ingredient 2", "Spice");

            // ✅ Create the Recipe
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
                ProteinPerRecipe = 15,
            };

            var content = new StringContent(JsonSerializer.Serialize(newRecipe), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/recipes", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response for CreateRecipe: {responseContent}"); // Debugging line

            var createdRecipe = JsonSerializer.Deserialize<RecipeDto>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (createdRecipe == null || createdRecipe.Id == Guid.Empty)
                throw new Exception($"❌ Failed to retrieve a valid Recipe ID from response: {responseContent}");

            // ✅ Now, create 2 measurements linked to the recipe
            await CreateMeasurement(createdRecipe.Id, ingredient1Id, 200);
            await CreateMeasurement(createdRecipe.Id, ingredient2Id, 150);

            return createdRecipe.Id;
        }


        // 🔹 Helper method: Get an Recipe by ID
        private async Task<RecipeDto> GetRecipe(Guid id)
        {
            var response = await _client.GetAsync($"/api/recipes/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) 
                throw new Exception($"❌ Recipe with ID {id} not found!"); // ✅ Clear error message

            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<RecipeDto>(
                await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // 🔹 Helper method: Update an Recipe
        private async Task UpdateRecipe(Guid id)
        {
            var updatedRecipe = new RecipeDto
            {
                Name = "Updated Recipe",
                RecipeCategory = "Soup",
                ServingsPerRecipe = 10,
                Instructions = new List<string> { "Step 1: Chop veggies", "Step 2: Boil water", "Step 3: Add ingredients" },                
                AppUserId = _testUserId,
                TotalPrice = 100m,
                PricePerServing = 10m,
                CaloriesPerRecipe = 200,
                CarbsPerRecipe = 40,
                FatPerRecipe = 10,
                ProteinPerRecipe = 15,
            };

            var content = new StringContent(JsonSerializer.Serialize(updatedRecipe), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/recipes/{id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // ✅ Ensure success response
        }

        // 🔹 Helper method: Delete an Recipe
        private async Task DeleteRecipe(Guid id)
        {
            var response = await _client.DeleteAsync($"/api/recipes/{id}");
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"❌ DeleteRecipe failed! Expected OK but got: {response.StatusCode}");
            }

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // ✅ Ensure success response
        }

        // ✅ **Main Chained Test**
        [Fact]
        public async Task FullRecipeLifecycle_ShouldWorkAsExpected()
        {
            // 🔹 Step 1: Create Recipe
            var createdId = await CreateRecipe();
            Assert.NotEqual(Guid.Empty, createdId);

            // 🔹 Step 2: Fetch and Validate
            var fetchedRecipe = await GetRecipe(createdId);
            Assert.NotNull(fetchedRecipe);
            Assert.Equal("Test Recipe", fetchedRecipe.Name);

            // 🔹 Step 3: Update Recipe
            await UpdateRecipe(createdId);

            // 🔹 Step 4: Fetch Updated Recipe
            var updatedRecipe = await GetRecipe(createdId);
            Assert.NotNull(updatedRecipe);
            Assert.Equal("Updated Recipe", updatedRecipe.Name);

            // 🔹 Step 5: Delete Recipe
            await DeleteRecipe(createdId);

            // 🔹 Step 6: Ensure it's gone
            var response = await _client.GetAsync($"/api/recipes/{createdId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); // ✅ Recipe should be gone

        }
    }
}
