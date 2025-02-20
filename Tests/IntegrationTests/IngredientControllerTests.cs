using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Ingredients;
using Tests.TestUtilities;

namespace DishCraft.Tests.IntegrationTests
{
    public class IngredientControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly string _testUserId = "746d6986-cbf9-4221-8350-21f7daa42c7b";

        public IngredientControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            // ✅ Set authentication header to simulate an authenticated user
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestAuth");
        }

        // 🔹 Helper method: Create Ingredient and return its ID
         private async Task<Guid> CreateIngredient()
        {
            var newIngredient = new IngredientDto
            {
                Name = "Test Ingredient",
                PricePerPackage = 4.99m,
                MeasurementsPerPackage = 1000,
                Category = "Baking",
                MeasuredIn = "Weight",
                Calories = 200,
                Carbs = 40,
                Fat = 10,
                Protein = 15,
                AppUserId = _testUserId
            };

            var content = new StringContent(JsonSerializer.Serialize(newIngredient), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/ingredients", content);

            // ✅ Check response code and deserialize properly
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdIngredient = JsonSerializer.Deserialize<IngredientDto>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (createdIngredient == null || createdIngredient.Id == Guid.Empty)
            {
                throw new Exception($"❌ Failed to retrieve a valid Ingredient ID from response: {responseContent}");
            }

            return createdIngredient.Id;
        }


        // 🔹 Helper method: Get an Ingredient by ID
        private async Task<IngredientDto> GetIngredient(Guid id)
        {
            var response = await _client.GetAsync($"/api/ingredients/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<IngredientDto>(
                await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // 🔹 Helper method: Update an Ingredient
        private async Task UpdateIngredient(Guid id)
        {
            var updatedIngredient = new IngredientDto
            {
                Id = id,
                Name = "Updated Ingredient",
                PricePerPackage = 5.99m,
                MeasurementsPerPackage = 500,
                Category = "Baking",
                MeasuredIn = "Weight",
                Calories = 100,
                Carbs = 20,
                Fat = 5,
                Protein = 10,
                AppUserId = _testUserId
            };

            var content = new StringContent(JsonSerializer.Serialize(updatedIngredient), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/ingredients/{id}", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // ✅ Ensure success response
        }

        // 🔹 Helper method: Delete an Ingredient
        private async Task DeleteIngredient(Guid id)
        {
            var response = await _client.DeleteAsync($"/api/ingredients/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // ✅ Ensure success response
        }

        // ✅ **Main Chained Test**
        [Fact]
        public async Task FullIngredientLifecycle_ShouldWorkAsExpected()
        {
            // 🔹 Step 1: Create Ingredient
            var createdId = await CreateIngredient();
            Assert.NotEqual(Guid.Empty, createdId);

            // 🔹 Step 2: Fetch and Validate
            var fetchedIngredient = await GetIngredient(createdId);
            Assert.NotNull(fetchedIngredient);
            Assert.Equal("Test Ingredient", fetchedIngredient.Name);

            // 🔹 Step 3: Update Ingredient
            await UpdateIngredient(createdId);

            // 🔹 Step 4: Fetch Updated Ingredient
            var updatedIngredient = await GetIngredient(createdId);
            Assert.NotNull(updatedIngredient);
            Assert.Equal("Updated Ingredient", updatedIngredient.Name);

            // 🔹 Step 5: Delete Ingredient
            await DeleteIngredient(createdId);

            // 🔹 Step 6: Ensure it's gone
            var deletedIngredient = await GetIngredient(createdId);
            Assert.Null(deletedIngredient);
        }
    }
}
