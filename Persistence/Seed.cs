using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Persistence;
public class Seed
{
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
    {
        // Step 1: Seed Users
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

        // Fetch all users
        var appUsers = userManager.Users.ToList();

        // Step 2: Seed Ingredients
        if (!context.Ingredients.Any())
        {
            var ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Id = Guid.NewGuid(), Name = "Flour", Category = Categories.IngredientCategory.Baking,
                    PricePerPackage = 2.50m, MeasurementsPerPackage = 1000
                },
                new Ingredient
                {
                    Id = Guid.NewGuid(), Name = "Milk", Category = Categories.IngredientCategory.Dairy,
                    PricePerPackage = 1.20m, MeasurementsPerPackage = 1000
                },
                new Ingredient
                {
                    Id = Guid.NewGuid(), Name = "Eggs", Category = Categories.IngredientCategory.Dairy,
                    PricePerPackage = 3.00m, MeasurementsPerPackage = 12
                },
                new Ingredient
                {
                    Id = Guid.NewGuid(), Name = "Sugar", Category = Categories.IngredientCategory.Baking,
                    PricePerPackage = 1.50m, MeasurementsPerPackage = 500
                }
            };

            int userIndex = 0;
            foreach (var ingredient in ingredients)
            {
                var user = appUsers[userIndex];
                ingredient.AppUserId = user.Id;
                ingredient.AppUser = user;
                userIndex = (userIndex + 1) % appUsers.Count; // Rotate users
            }

            await context.Ingredients.AddRangeAsync(ingredients);
            await context.SaveChangesAsync();
        }

        // Step 3: Seed Measurement Units and Nutritional Info
        foreach (var ingredient in context.Ingredients)
        {
            if (!context.MeasurementUnits.Any(mu => mu.IngredientId == ingredient.Id))
            {
                await context.MeasurementUnits.AddAsync(new MeasurementUnit
                {
                    IngredientId = ingredient.Id,
                    MeasuredIn = MeasuredIn.Weight,
                    WeightUnit = WeightUnit.Kilograms
                });
            }

            if (!context.NutritionalInfos.Any(ni => ni.IngredientId == ingredient.Id))
            {
                await context.NutritionalInfos.AddAsync(new NutritionalInfo
                {
                    IngredientId = ingredient.Id,
                    Calories = 364, // Example
                    Carbs = 76,
                    Fat = 1,
                    Protein = 10
                });
            }
        }

        await context.SaveChangesAsync();

        // Step 4: Seed Recipes
        if (!context.Recipes.Any())
        {
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
                    },
                    AppUserId = appUsers.First(u => u.UserName == "bob").Id
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
                    },
                    AppUserId = appUsers.First(u => u.UserName == "jane").Id
                }
            };

            await context.Recipes.AddRangeAsync(recipes);
            await context.SaveChangesAsync();
        }

        // Step 5: Seed Measurements
        if (!context.Measurements.Any())
        {
            var recipes = context.Recipes.ToList();
            var ingredients = context.Ingredients.ToList();

            var measurements = new List<Measurement>
            {
                new Measurement
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipes.First(r => r.Name == "Pancakes").Id,
                    IngredientId = ingredients.First(i => i.Name == "Flour").Id,
                    Amount = 200
                },
                new Measurement
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipes.First(r => r.Name == "Pancakes").Id,
                    IngredientId = ingredients.First(i => i.Name == "Milk").Id,
                    Amount = 300
                },
                new Measurement
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipes.First(r => r.Name == "Scrambled Eggs").Id,
                    IngredientId = ingredients.First(i => i.Name == "Eggs").Id,
                    Amount = 2
                }
            };

            await context.Measurements.AddRangeAsync(measurements);
            await context.SaveChangesAsync();
        }
    }
}


/*
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Persistence;

public class Seed
{
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
    {
        // Step 1: Seed Users
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

        // Step 2: Seed Ingredients
        var appUsers = userManager.Users.ToList(); // Fetch all users
        var appUsersDictionary = appUsers.ToDictionary(u => u.UserName, u => u.Id);

        var ingredients = new List<Ingredient>
        {
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Flour", Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 2.50m, MeasurementsPerPackage = 1000,
                AppUserId = appUsersDictionary["bob"]
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Milk", Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 1.20m, MeasurementsPerPackage = 1000,
                AppUserId = appUsersDictionary["bob"]
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Eggs", Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 3.00m, MeasurementsPerPackage = 12,
                AppUserId = appUsersDictionary["jane"]
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Sugar", Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 1.50m, MeasurementsPerPackage = 500,
                AppUserId = appUsersDictionary["bob"]
            }
        };

        foreach (var ingredient in ingredients)
        {
            Console.WriteLine($"Assigning {ingredient.Name} to UserId: {ingredient.AppUserId}");
            if (!context.Ingredients.Any(i => i.Name == ingredient.Name && i.AppUserId == ingredient.AppUserId))
            {
                await context.Ingredients.AddAsync(ingredient);
            }
        }

        await context.SaveChangesAsync();

        // Step 3: Seed Measurement Units and Nutritional Info
        foreach (var ingredient in ingredients)
        {
            if (!context.MeasurementUnits.Any(mu => mu.IngredientId == ingredient.Id))
            {
                await context.MeasurementUnits.AddAsync(new MeasurementUnit
                {
                    IngredientId = ingredient.Id,
                    MeasuredIn = MeasuredIn.Weight,
                    WeightUnit = WeightUnit.Kilograms
                });
            }

            if (!context.NutritionalInfos.Any(ni => ni.IngredientId == ingredient.Id))
            {
                await context.NutritionalInfos.AddAsync(new NutritionalInfo
                {
                    IngredientId = ingredient.Id,
                    Calories = 364, // Example
                    Carbs = 76,
                    Fat = 1,
                    Protein = 10
                });
            }
        }

        await context.SaveChangesAsync();

        // Step 4: Seed Recipes with User Ownership
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
                },
                AppUserId = appUsersDictionary["bob"]
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
                },
                AppUserId = appUsersDictionary["jane"]
            }
        };

        foreach (var recipe in recipes)
        {
            if (!context.Recipes.Any(r => r.Name == recipe.Name && r.AppUserId == recipe.AppUserId))
            {
                await context.Recipes.AddAsync(recipe);
            }
        }

        await context.SaveChangesAsync(); // Save recipes to the database

        // Debugging logs
        foreach (var ingredient in ingredients)
        {
            Console.WriteLine($"Ingredient: {ingredient.Name}, AppUserId: {ingredient.AppUserId}");
        }

        foreach (var user in appUsers)
        {
            Console.WriteLine($"Username: {user.UserName}, UserId: {user.Id}");
        }

        var pancakeRecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id;
        var scrambledEggsRecipeId = context.Recipes.First(r => r.Name == "Scrambled Eggs").Id;

        Console.WriteLine($"Pancake Recipe ID: {pancakeRecipeId}");
        Console.WriteLine($"Scrambled Eggs Recipe ID: {scrambledEggsRecipeId}");

        var ingredientsList = context.Ingredients.ToList();
        Console.WriteLine("Ingredients in the database:");
        foreach (var dbIngredient in ingredientsList)
        {
            Console.WriteLine($"Ingredient: {dbIngredient.Name}, AppUserId: {dbIngredient.AppUserId}");
        }

        var measurements = new List<Measurement>
        {
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = pancakeRecipeId,
                IngredientId = ingredientsList.First(i =>
                                   i.Name == "Flour" && i.AppUserId == appUsersDictionary["bob"]).Id,
                Amount = 200
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = pancakeRecipeId,
                IngredientId = ingredientsList.First(i =>
                                   i.Name == "Milk" && i.AppUserId == appUsersDictionary["bob"]).Id,
                Amount = 300
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = scrambledEggsRecipeId,
                IngredientId = ingredientsList.First(i =>
                                   i.Name == "Eggs" && i.AppUserId == appUsersDictionary["jane"]).Id,
                Amount = 2
            }
        };

        foreach (var measurement in measurements)
        {
            if (!context.Measurements.Any(m =>
                    m.RecipeId == measurement.RecipeId && m.IngredientId == measurement.IngredientId))
            {
                await context.Measurements.AddAsync(measurement);
            }
        }

        await context.SaveChangesAsync();

        // Step 6: Add Ingredients and Recipes to Users
        foreach (var user in appUsers)
        {
            user.Ingredients = ingredientsList.Where(i => i.AppUserId == user.Id).ToList();
            user.Recipes = context.Recipes.Where(r => r.AppUserId == user.Id).ToList();
        }

        await context.SaveChangesAsync();
    }
}




/*
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Persistence;

public class Seed
{
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
    {
        // Step 1: Seed Users
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

        // Step 2: Seed Ingredients
        var appUsers = userManager.Users.ToList(); // Fetch all users
        var ingredients = new List<Ingredient>
        {
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Flour", Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 2.50m, MeasurementsPerPackage = 1000
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Milk", Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 1.20m, MeasurementsPerPackage = 1000
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Eggs", Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 3.00m, MeasurementsPerPackage = 12
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Sugar", Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 1.50m, MeasurementsPerPackage = 500
            }
        };

        int userIndex = 0;
        foreach (var ingredient in ingredients)
        {
            var user = appUsers[userIndex];
            ingredient.AppUserId = user.Id;
            ingredient.AppUser = user;
            Console.WriteLine($"Assigning {ingredient.Name} to UserId: {user.Id}");
            userIndex = (userIndex + 1) % appUsers.Count; // Rotate users
        }

        foreach (var ingredient in ingredients)
        {
            if (!context.Ingredients.Any(i => i.Name == ingredient.Name && i.AppUserId == ingredient.AppUserId))
            {
                await context.Ingredients.AddAsync(ingredient);
            }
        }

        await context.SaveChangesAsync();

        // Step 3: Seed Measurement Units and Nutritional Info
        foreach (var ingredient in ingredients)
        {
            if (!context.MeasurementUnits.Any(mu => mu.IngredientId == ingredient.Id))
            {
                await context.MeasurementUnits.AddAsync(new MeasurementUnit
                {
                    IngredientId = ingredient.Id,
                    MeasuredIn = MeasuredIn.Weight,
                    WeightUnit = WeightUnit.Kilograms
                });
            }

            if (!context.NutritionalInfos.Any(ni => ni.IngredientId == ingredient.Id))
            {
                await context.NutritionalInfos.AddAsync(new NutritionalInfo
                {
                    IngredientId = ingredient.Id,
                    Calories = 364, // Example
                    Carbs = 76,
                    Fat = 1,
                    Protein = 10
                });
            }
        }

        await context.SaveChangesAsync();

        // Step 4: Seed Recipes with User Ownership
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
                },
                AppUserId = appUsers.First(u => u.UserName == "bob").Id
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
                },
                AppUserId = appUsers.First(u => u.UserName == "jane").Id
            }
        };

        foreach (var recipe in recipes)
        {
            if (!context.Recipes.Any(r => r.Name == recipe.Name && r.AppUserId == recipe.AppUserId))
            {
                await context.Recipes.AddAsync(recipe);
            }
        }

        await context.SaveChangesAsync(); // Save recipes to the database

        // Start of updated code for seeding measurements
        // Debugging logs
        foreach (var ingredient in ingredients)
        {
            Console.WriteLine($"Ingredient: {ingredient.Name}, AppUserId: {ingredient.AppUserId}");
        }

        foreach (var user in appUsers)
        {
            Console.WriteLine($"Username: {user.UserName}, UserId: {user.Id}");
        }

        var pancakeRecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id;
        var scrambledEggsRecipeId = context.Recipes.First(r => r.Name == "Scrambled Eggs").Id;

        Console.WriteLine($"Pancake Recipe ID: {pancakeRecipeId}");
        Console.WriteLine($"Scrambled Eggs Recipe ID: {scrambledEggsRecipeId}");

        if (!recipes.Any() || !ingredients.Any())
        {
            throw new InvalidOperationException("Recipes or ingredients list is empty.");
        }

        var appUsersDictionary = appUsers.ToDictionary(u => u.UserName, u => u.Id);
        var ingredientsList = context.Ingredients.ToList();
        Console.WriteLine("Ingredients in the database:");
        foreach (var dbIngredient in ingredientsList)
        {
            Console.WriteLine($"Ingredient: {dbIngredient.Name}, AppUserId: {dbIngredient.AppUserId}");
        }
        if (!ingredientsList.Any())
        {
            throw new InvalidOperationException("No ingredients found in the database. Check the seeding logic.");
        }

        var measurements = new List<Measurement>
        {
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = pancakeRecipeId,
                IngredientId = ingredientsList.FirstOrDefault(i =>
                                   i.Name == "Flour" && i.AppUserId == appUsersDictionary["bob"])?.Id 
                               ?? throw new InvalidOperationException("Flour for bob not found"),
                Amount = 200
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = pancakeRecipeId,
                IngredientId = ingredientsList.FirstOrDefault(i =>
                                   i.Name == "Milk" && i.AppUserId == appUsersDictionary["bob"])?.Id 
                               ?? throw new InvalidOperationException("Milk for bob not found"),
                Amount = 300
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = scrambledEggsRecipeId,
                IngredientId = ingredientsList.FirstOrDefault(i =>
                                   i.Name == "Eggs" && i.AppUserId == appUsersDictionary["jane"])?.Id 
                               ?? throw new InvalidOperationException("Eggs for jane not found"),
                Amount = 2
            }
        };

        foreach (var measurement in measurements)
        {
            if (!context.Measurements.Any(m =>
                    m.RecipeId == measurement.RecipeId && m.IngredientId == measurement.IngredientId))
            {
                await context.Measurements.AddAsync(measurement);
            }
        }

        await context.SaveChangesAsync();

        // Step 6: Add Ingredients and Recipes to Users
        foreach (var user in appUsers)
        {
            user.Ingredients = ingredientsList.Where(i => i.AppUserId == user.Id).ToList();
            user.Recipes = context.Recipes.Where(r => r.AppUserId == user.Id).ToList();
        }

        await context.SaveChangesAsync();
    }
}


/*
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Persistence;

public class Seed
{
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
    {
        // Step 1: Seed Users
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

        // Step 2: Seed Ingredients
        var appUsers = userManager.Users.ToList(); // Fetch all users
        var ingredients = new List<Ingredient>
        {
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Flour", Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 2.50m, MeasurementsPerPackage = 1000
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Milk", Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 1.20m, MeasurementsPerPackage = 1000
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Eggs", Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 3.00m, MeasurementsPerPackage = 12
            },
            new Ingredient
            {
                Id = Guid.NewGuid(), Name = "Sugar", Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 1.50m, MeasurementsPerPackage = 500
            }
        };

        int userIndex = 0;
        foreach (var ingredient in ingredients)
        {
            var user = appUsers[userIndex];
            ingredient.AppUserId = user.Id;
            ingredient.AppUser = user;

            userIndex = (userIndex + 1) % appUsers.Count; // Rotate users
        }

        foreach (var ingredient in ingredients)
        {
            if (!context.Ingredients.Any(i => i.Name == ingredient.Name && i.AppUserId == ingredient.AppUserId))
            {
                await context.Ingredients.AddAsync(ingredient);
            }
        }

        await context.SaveChangesAsync();

        // Step 3: Seed Measurement Units and Nutritional Info
        foreach (var ingredient in ingredients)
        {
            if (!context.MeasurementUnits.Any(mu => mu.IngredientId == ingredient.Id))
            {
                await context.MeasurementUnits.AddAsync(new MeasurementUnit
                {
                    IngredientId = ingredient.Id,
                    MeasuredIn = MeasuredIn.Weight,
                    WeightUnit = WeightUnit.Kilograms
                });
            }

            if (!context.NutritionalInfos.Any(ni => ni.IngredientId == ingredient.Id))
            {
                await context.NutritionalInfos.AddAsync(new NutritionalInfo
                {
                    IngredientId = ingredient.Id,
                    Calories = 364, // Example
                    Carbs = 76,
                    Fat = 1,
                    Protein = 10
                });
            }
        }

        await context.SaveChangesAsync();

        // Step 4: Seed Recipes with User Ownership
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
                },
                AppUserId = appUsers.First(u => u.UserName == "bob").Id
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
                },
                AppUserId = appUsers.First(u => u.UserName == "jane").Id
            }
        };

        foreach (var recipe in recipes)
        {
            if (!context.Recipes.Any(r => r.Name == recipe.Name && r.AppUserId == recipe.AppUserId))
            {
                await context.Recipes.AddAsync(recipe);
            }
        }

        await context.SaveChangesAsync(); // Save recipes to the database

// Start of updated code for seeding measurements
// Fetch all users into memory first
        var appUsersDictionary = appUsers.ToDictionary(u => u.UserName, u => u.Id);

// Fetch Recipes and Ingredients into memory
        var pancakeRecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id;
        var scrambledEggsRecipeId = context.Recipes.First(r => r.Name == "Scrambled Eggs").Id;

        var ingredientsList = context.Ingredients.ToList();

// Create Measurements
        var measurements = new List<Measurement>
        {
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = pancakeRecipeId,
                IngredientId = ingredientsList.First(i =>
                    i.Name == "Flour" && i.AppUserId == appUsersDictionary["bob"]).Id,
                Amount = 200
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = pancakeRecipeId,
                IngredientId = ingredientsList.First(i =>
                    i.Name == "Milk" && i.AppUserId == appUsersDictionary["bob"]).Id,
                Amount = 300
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = scrambledEggsRecipeId,
                IngredientId = ingredientsList.First(i =>
                    i.Name == "Eggs" && i.AppUserId == appUsersDictionary["jane"]).Id,
                Amount = 2
            }
        };

// Avoid duplicate measurements
        foreach (var measurement in measurements)
        {
            if (!context.Measurements.Any(m =>
                    m.RecipeId == measurement.RecipeId && m.IngredientId == measurement.IngredientId))
            {
                await context.Measurements.AddAsync(measurement);
            }
        }

        await context.SaveChangesAsync();

// Step 6: Add Ingredients and Recipes to Users
        foreach (var user in appUsers)
        {
            user.Ingredients = ingredientsList.Where(i => i.AppUserId == user.Id).ToList();
            user.Recipes = context.Recipes.Where(r => r.AppUserId == user.Id).ToList();
        }

        await context.SaveChangesAsync();

    }
}

/*
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Persistence;

public class Seed
{
    public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
    {
        // Step 1: Seed Users
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

        // Get users from the database
        var appUsers = userManager.Users.ToList();

        // Step 2: Seed Ingredients with User Ownership
        var ingredientsList = new List<Ingredient>
        {
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "Flour",
                Category = Categories.IngredientCategory.Baking,
                PricePerPackage = 2.50m,
                MeasurementsPerPackage = 1000,
                AppUserId = appUsers.First(u => u.UserName == "bob").Id
            },
            new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = "Milk",
                Category = Categories.IngredientCategory.Dairy,
                PricePerPackage = 1.20m,
                MeasurementsPerPackage = 1000,
                AppUserId = appUsers.First(u => u.UserName == "tom").Id
            },
            // Add more ingredients with user associations
        };

        foreach (var ingredient in ingredients)
        {
            if (!context.Ingredients.Any(i => i.Name == ingredient.Name && i.AppUserId == ingredient.AppUser.Id))
            {
                await context.Ingredients.AddAsync(ingredient);
            }
        }
        await context.SaveChangesAsync();

        // Step 3: Seed Measurement Units and Nutritional Info
        foreach (var ingredient in ingredients)
        {
            if (!context.MeasurementUnits.Any(mu => mu.IngredientId == ingredient.Id))
            {
                await context.MeasurementUnits.AddAsync(new MeasurementUnit
                {
                    IngredientId = ingredient.Id,
                    MeasuredIn = MeasuredIn.Weight,
                    WeightUnit = WeightUnit.Kilograms // Example
                });
            }

            if (!context.NutritionalInfos.Any(ni => ni.IngredientId == ingredient.Id))
            {
                await context.NutritionalInfos.AddAsync(new NutritionalInfo
                {
                    IngredientId = ingredient.Id,
                    Calories = 364, // Example
                    Carbs = 76,
                    Fat = 1,
                    Protein = 10
                });
            }
        }
        await context.SaveChangesAsync();

        // Step 4: Seed Recipes with User Ownership
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
                },
                AppUserId = appUsers.First(u => u.UserName == "bob").Id
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
                },
                AppUserId = appUsers.First(u => u.UserName == "jane").Id
            }
        };

        foreach (var recipe in recipes)
        {
            if (!context.Recipes.Any(r => r.Name == recipe.Name && r.AppUserId == recipe.AppUserId))
            {
                await context.Recipes.AddAsync(recipe);
            }
        }
        await context.SaveChangesAsync();

        // Step 5: Seed Measurements
        var measurements = new List<Measurement>
        {
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Flour" && i.AppUserId == appUsers.First(u => u.UserName == "bob").Id).Id,
                Amount = 200
            },
            new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = context.Recipes.First(r => r.Name == "Pancakes").Id,
                IngredientId = context.Ingredients.First(i => i.Name == "Milk" && i.AppUserId == appUsers.First(u => u.UserName == "bob").Id).Id,
                Amount = 300
            }
            // Add more measurements as needed
        };

        foreach (var measurement in measurements)
        {
            if (!context.Measurements.Any(m => m.RecipeId == measurement.RecipeId && m.IngredientId == measurement.IngredientId))
            {
                await context.Measurements.AddAsync(measurement);
            }
        }
        await context.SaveChangesAsync();

        // Step 6: Add Ingredients and Recipes to Users
        foreach (var user in appUsers)
        {
            user.Ingredients = context.Ingredients.Where(i => i.AppUserId == user.Id).ToList();
            user.Recipes = context.Recipes.Where(r => r.AppUserId == user.Id).ToList();
        }
        await context.SaveChangesAsync();
    }
}



/*
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
#4#
#3#
#2#
#1#
*/
