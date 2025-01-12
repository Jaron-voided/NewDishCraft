using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Persistence;

public class Seed
{
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            var users = new List<AppUser>
            {
                new AppUser { DisplayName = "Bob", UserName = "bob", Email = "bob@test.com" },
                new AppUser { DisplayName = "Tom", UserName = "tom", Email = "tom@test.com" },
                new AppUser { DisplayName = "Jane", UserName = "jane", Email = "jane@test.com" }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
        // Seed Ingredients
        var ingredients = new List<Ingredient>
        {
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "Flour",
                Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 2.50m,
                MeasurementsPerPackage = 1000
            },
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "Milk",
                Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 1.20m,
                MeasurementsPerPackage = 1000
            },
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "Eggs",
                Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 3.00m,
                MeasurementsPerPackage = 12
            },
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "Sugar",
                Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 1.50m,
                MeasurementsPerPackage = 500
            }
        };

        foreach (var ingredient in ingredients)
        {
            if (!context.Ingredients.Any(i => i.Name == ingredient.Name))
            {
                await context.Ingredients.AddAsync(ingredient);
            }
        }
        await context.SaveChangesAsync();

        // Seed MeasurementUnits
        var measurementUnits = new List<MeasurementUnit>
        {
            new MeasurementUnit
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Flour").Id,
                MeasuredIn = MeasuredIn.Weight,
                WeightUnit = WeightUnit.Kilograms
            },
            new MeasurementUnit
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Milk").Id,
                MeasuredIn = MeasuredIn.Volume,
                VolumeUnit = VolumeUnit.Liters
            },
            new MeasurementUnit
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Eggs").Id,
                MeasuredIn = MeasuredIn.Each
            },
            new MeasurementUnit
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Sugar").Id,
                MeasuredIn = MeasuredIn.Weight,
                WeightUnit = WeightUnit.Pounds
            }
        };

        foreach (var unit in measurementUnits)
        {
            if (!context.MeasurementUnits.Any(mu => mu.IngredientId == unit.IngredientId))
            {
                await context.MeasurementUnits.AddAsync(unit);
            }
        }
        await context.SaveChangesAsync();

        // Seed NutritionalInfo
        var nutritionalInfos = new List<NutritionalInfo>
        {
            new NutritionalInfo
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Flour").Id,
                Calories = 364,
                Carbs = 76,
                Fat = 1,
                Protein = 10
            },
            new NutritionalInfo
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Milk").Id,
                Calories = 42,
                Carbs = 5,
                Fat = 1,
                Protein = 3
            },
            new NutritionalInfo
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Eggs").Id,
                Calories = 155,
                Carbs = 1,
                Fat = 11,
                Protein = 13
            },
            new NutritionalInfo
            {
                IngredientId = context.Ingredients.First(i => i.Name == "Sugar").Id,
                Calories = 387,
                Carbs = 100,
                Fat = 0,
                Protein = 0
            }
        };

        foreach (var nutrition in nutritionalInfos)
        {
            if (!context.NutritionalInfos.Any(ni => ni.IngredientId == nutrition.IngredientId))
            {
                await context.NutritionalInfos.AddAsync(nutrition);
            }
        }
        await context.SaveChangesAsync();

        // Seed Recipes
        var recipes = new List<Recipe>
        {
            new Recipe
            {
                Id = Guid.NewGuid(),
                Name = "Pancakes",
                RecipeCategory = Categories.RecipeCategory.Breakfast,
                ServingsPerRecipe = 4,
                Instructions = new List<string>
                {
                    "Mix flour, milk, eggs, and sugar.",
                    "Whisk until smooth.",
                    "Cook on a hot griddle until golden brown on both sides."
                }
            },
            new Recipe
            {
                Id = Guid.NewGuid(),
                Name = "Scrambled Eggs",
                RecipeCategory = Categories.RecipeCategory.Breakfast,
                ServingsPerRecipe = 2,
                Instructions = new List<string>
                {
                    "Crack eggs into a bowl.",
                    "Add a splash of milk and whisk.",
                    "Cook on low heat while stirring until fluffy."
                }
            },
            new Recipe
            {
                Id = Guid.NewGuid(),
                Name = "Sugar Cookies",
                RecipeCategory = Categories.RecipeCategory.Dessert,
                ServingsPerRecipe = 12,
                Instructions = new List<string>
                {
                    "Mix flour, sugar, and eggs.",
                    "Shape dough into cookies.",
                    "Bake in a preheated oven at 350°F (175°C) for 10-12 minutes."
                }
            }
        };

        foreach (var recipe in recipes)
        {
            if (!context.Recipes.Any(r => r.Name == recipe.Name))
            {
                await context.Recipes.AddAsync(recipe);
            }
        }
        await context.SaveChangesAsync();

        // Seed Measurements
        var measurements = new List<Measurement>
        {
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Flour").Id,
                Amount = 200
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Milk").Id,
                Amount = 300
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Eggs").Id,
                Amount = 2
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Sugar").Id,
                Amount = 50
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Scrambled Eggs").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Eggs").Id,
                Amount = 4
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Scrambled Eggs").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Milk").Id,
                Amount = 50
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Sugar Cookies").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Flour").Id,
                Amount = 300
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Sugar Cookies").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Sugar").Id,
                Amount = 150
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Sugar Cookies").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Eggs").Id,
                Amount = 1
            }
        };

        foreach (var measurement in measurements)
        {
            if (!context.Measurements.Any(m => m.RecipeId == measurement.RecipeId && m.IngredientId == measurement.IngredientId))
            {
                await context.Measurements.AddAsync(measurement);
            }
        }
        await context.SaveChangesAsync();
    }
}
