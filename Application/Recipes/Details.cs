using Application.Core;
using Application.Measurements;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Recipes;

public class Details
{
    public class Query : IRequest<Result<RecipeDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<RecipeDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<RecipeDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            try 
            {
                if (request.Id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid Recipe ID received: {RecipeId}", request.Id);
                    return Result<RecipeDto>.Failure("Invalid Recipe ID");
                }

                var recipe = await _context.Recipes
                    //.Where(r => r.Id == request.Id)
                    .Include(r => r.Measurements)
                    .ThenInclude(m => m.Ingredient)
                    .ThenInclude(i => i.Nutrition)
                    .FirstOrDefaultAsync(r => r.Id == request.Id);
                    //.FirstOrDefaultAsync(cancellationToken);

                if (recipe == null)
                {
                    _logger.LogWarning("Recipe with ID {RecipeId} not found", request.Id);
                    return Result<RecipeDto>.Failure("Recipe not found");
                }

                // Manual mapping with calculations
                var recipeDto = new RecipeDto
                {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    RecipeCategory = recipe.RecipeCategory.ToString(),
                    ServingsPerRecipe = recipe.ServingsPerRecipe,
                    AppUserId = recipe.AppUserId,
                    Instructions = recipe.Instructions,

                    TotalPrice = recipe.Measurements?.Sum(m =>
                        m.Amount * (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage)) ?? 0m,

                    PricePerServing = recipe.ServingsPerRecipe > 0
                        ? (recipe.Measurements?.Sum(m =>
                               m.Amount * (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage))
                           / recipe.ServingsPerRecipe) ?? 0m
                        : 0m,

                    CaloriesPerRecipe = recipe.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Calories) ?? 0,

                    CarbsPerRecipe = recipe.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Carbs) ?? 0,

                    FatPerRecipe = recipe.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Fat) ?? 0,

                    ProteinPerRecipe = recipe.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Protein) ?? 0,
                    
                    Measurements = recipe.Measurements.Select(m => new MeasurementDto 
                    {
                        Id = m.Id,
                        RecipeId = m.RecipeId,
                        IngredientId = m.IngredientId,
                        Amount = m.Amount,
                        RecipeName = recipe.Name,
                        IngredientName = m.Ingredient.Name,
                        IngredientPricePerMeasurement = m.Ingredient.MeasurementsPerPackage > 0 
                            ? (double)(m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage) 
                            : 0,
                        IngredientCalories = m.Ingredient.Nutrition?.Calories ?? 0,
                        IngredientCarbs = m.Ingredient.Nutrition?.Carbs ?? 0,
                        IngredientFat = m.Ingredient.Nutrition?.Fat ?? 0,
                        IngredientProtein = m.Ingredient.Nutrition?.Protein ?? 0,
                        PricePerAmount = m.Amount * (m.Ingredient.MeasurementsPerPackage > 0 
                            ? (decimal)(m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage) 
                            : 0m),
                        CaloriesPerAmount = (double)m.Amount * (m.Ingredient.Nutrition?.Calories ?? 0),
                        CarbsPerAmount = (double)m.Amount * (m.Ingredient.Nutrition?.Carbs ?? 0),
                        FatPerAmount = (double)m.Amount * (m.Ingredient.Nutrition?.Fat ?? 0),
                        ProteinPerAmount = (double)m.Amount * (m.Ingredient.Nutrition?.Protein ?? 0)
                    }).ToList(),
                        /*Id = m.Id,
                        RecipeId = m.RecipeId,
                        IngredientId = m.IngredientId,
                        Amount = m.Amount,
                        IngredientName = m.Ingredient.Name,
                        IngredientCalories = m.Ingredient.Nutrition?.Calories ?? 0,
                        // ... other measurement properties
                    }).ToList() ?? new List<MeasurementDto>(),*/

                    Nutrition = new RecipeNutritionDto
                    {
                        RecipeId = recipe.Id,
                        TotalCalories = recipe.Measurements?.Sum(m => 
                            (double)m.Amount * m.Ingredient.Nutrition.Calories) ?? 0,
                        TotalCarbs = recipe.Measurements?.Sum(m => 
                            (double)m.Amount * m.Ingredient.Nutrition.Carbs) ?? 0,
                        TotalFat = recipe.Measurements?.Sum(m => 
                            (double)m.Amount * m.Ingredient.Nutrition.Fat) ?? 0,
                        TotalProtein = recipe.Measurements?.Sum(m => 
                            (double)m.Amount * m.Ingredient.Nutrition.Protein) ?? 0,
                        CaloriesPerServing = recipe.ServingsPerRecipe > 0 
                            ? (recipe.Measurements?.Sum(m => 
                                (double)m.Amount * m.Ingredient.Nutrition.Calories) / recipe.ServingsPerRecipe) ?? 0 
                            : 0,
                        CarbsPerServing = recipe.ServingsPerRecipe > 0 
                            ? (recipe.Measurements?.Sum(m => 
                                (double)m.Amount * m.Ingredient.Nutrition.Carbs) / recipe.ServingsPerRecipe) ?? 0 
                            : 0,
                        FatPerServing = recipe.ServingsPerRecipe > 0 
                            ? (recipe.Measurements?.Sum(m => 
                                (double)m.Amount * m.Ingredient.Nutrition.Fat) / recipe.ServingsPerRecipe) ?? 0 
                            : 0,
                        ProteinPerServing = recipe.ServingsPerRecipe > 0 
                            ? (recipe.Measurements?.Sum(m => 
                                (double)m.Amount * m.Ingredient.Nutrition.Protein) / recipe.ServingsPerRecipe) ?? 0 
                            : 0
                    }
                };

                // Logging
                _logger.LogInformation("Recipe details retrieved: {RecipeName}", recipeDto.Name);
                _logger.LogDebug("Recipe Nutrition - Total Calories: {TotalCalories}, Price Per Serving: {PricePerServing}", 
                    recipeDto.Nutrition?.TotalCalories, 
                    recipeDto.PricePerServing);

                return Result<RecipeDto>.Success(recipeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching recipe details");
                return Result<RecipeDto>.Failure("An error occurred while fetching recipe details");
            }
        }
    }
}