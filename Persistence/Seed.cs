using Domain.Enums;
using Domain.Models;

namespace Persistence;

public class Seed
{
    public static async Task SeedData(DataContext context)
    {
        // Check if any data already exists
        if (context.Ingredients.Any() || context.Recipes.Any() || context.Measurements.Any())
            return;

        // Seed Ingredients
        var flour = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Flour",
            Category = Categories.IngredientCategory.Baking,
            PricePerPackage = 2.50m,
            MeasurementsPerPackage = 1000
        };

        var milk = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Milk",
            Category = Categories.IngredientCategory.Dairy,
            PricePerPackage = 1.20m,
            MeasurementsPerPackage = 1000
        };

        var eggs = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Eggs",
            Category = Categories.IngredientCategory.Dairy,
            PricePerPackage = 3.00m,
            MeasurementsPerPackage = 12
        };

        var sugar = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Sugar",
            Category = Categories.IngredientCategory.Baking,
            PricePerPackage = 1.50m,
            MeasurementsPerPackage = 500
        };

        var ingredients = new List<Ingredient> { flour, milk, eggs, sugar };
        await context.Ingredients.AddRangeAsync(ingredients);
        await context.SaveChangesAsync(); // Save Ingredients to generate IDs

        // Seed MeasurementUnits
        var measurementUnits = new List<MeasurementUnit>
        {
            new MeasurementUnit
            {
                IngredientId = flour.Id,
                MeasuredIn = MeasuredIn.Weight,
                WeightUnit = WeightUnit.Kilograms
            },
            new MeasurementUnit
            {
                IngredientId = milk.Id,
                MeasuredIn = MeasuredIn.Volume,
                VolumeUnit = VolumeUnit.Liters
            },
            new MeasurementUnit
            {
                IngredientId = eggs.Id,
                MeasuredIn = MeasuredIn.Each
            },
            new MeasurementUnit
            {
                IngredientId = sugar.Id,
                MeasuredIn = MeasuredIn.Weight,
                WeightUnit = WeightUnit.Pounds
            }
        };
        await context.MeasurementUnits.AddRangeAsync(measurementUnits);

        // Seed NutritionalInfo
        var nutritionalInfos = new List<NutritionalInfo>
        {
            new NutritionalInfo
            {
                IngredientId = flour.Id,
                Calories = 364,
                Carbs = 76,
                Fat = 1,
                Protein = 10
            },
            new NutritionalInfo
            {
                IngredientId = milk.Id,
                Calories = 42,
                Carbs = 5,
                Fat = 1,
                Protein = 3
            },
            new NutritionalInfo
            {
                IngredientId = eggs.Id,
                Calories = 155,
                Carbs = 1,
                Fat = 11,
                Protein = 13
            },
            new NutritionalInfo
            {
                IngredientId = sugar.Id,
                Calories = 387,
                Carbs = 100,
                Fat = 0,
                Protein = 0
            }
        };
        await context.NutritionalInfos.AddRangeAsync(nutritionalInfos);
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
        await context.Recipes.AddRangeAsync(recipes);
        await context.SaveChangesAsync();

        // Seed Measurements
        var measurements = new List<Measurement>
        {
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[0].Id,
                IngredientId = flour.Id,
                Amount = 200
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[0].Id,
                IngredientId = milk.Id,
                Amount = 300
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[0].Id,
                IngredientId = eggs.Id,
                Amount = 2
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[0].Id,
                IngredientId = sugar.Id,
                Amount = 50
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[1].Id,
                IngredientId = eggs.Id,
                Amount = 4
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[1].Id,
                IngredientId = milk.Id,
                Amount = 50
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[2].Id,
                IngredientId = flour.Id,
                Amount = 300
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[2].Id,
                IngredientId = sugar.Id,
                Amount = 150
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = recipes[2].Id,
                IngredientId = eggs.Id,
                Amount = 1
            }
        };
        await context.Measurements.AddRangeAsync(measurements);
        await context.SaveChangesAsync();
    }
}
