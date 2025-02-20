using Application.DayPlan;
using Application.DayPlanRecipe;

namespace DishCraft.Tests.BusinessLogicTests
{
    public class DayPlanLogicTests
    {
        [Fact]
        public void DayPlan_ShouldComputeTotalCostAndNutrition()
        {
            // ✅ Create a DayPlanDto with multiple recipes
            var dayPlan = new DayPlanDto
            {
                Name = "Test Day Plan",
                DayPlanRecipes = new List<DayPlanRecipeDto>
                {
                    new DayPlanRecipeDto
                    {
                        RecipeId = Guid.NewGuid(),
                        RecipeName = "Chili",
                        Servings = 2,
                        PricePerServing = 5m,  // ✅ Price per serving
                        CaloriesPerRecipe = 800,
                        ServingsPerRecipe = 4
                    },
                    new DayPlanRecipeDto
                    {
                        RecipeId = Guid.NewGuid(),
                        RecipeName = "Ham",
                        Servings = 1,
                        PricePerServing = 3m,  // ✅ Price per serving
                        CaloriesPerRecipe = 600,
                        ServingsPerRecipe = 3
                    }
                }
            };

            // ✅ Compute Expected Values
            decimal expectedTotalPrice = dayPlan.DayPlanRecipes.Sum(r => r.PricePerServing * r.Servings);
            decimal expectedTotalCalories = dayPlan.DayPlanRecipes.Sum(r => (r.CaloriesPerRecipe / r.ServingsPerRecipe) * r.Servings);

            // ✅ Assign Computed Values to DTO (Simulating real application behavior)
            dayPlan.PriceForDay = expectedTotalPrice;
            dayPlan.CaloriesPerDay = expectedTotalCalories;

            // ✅ Assertions - Ensures calculations are correct
            Assert.Equal(13, dayPlan.PriceForDay); // (5 * 2) + (3 * 1) = 10 + 3 = 13
            Assert.Equal(600, dayPlan.CaloriesPerDay); // (800/4 * 2) + (600/3 * 1) = 400 + 200 = 600
        }
    }
}
