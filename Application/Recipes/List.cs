using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Core;
using Application.Interfaces;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Logging;

namespace Application.Recipes;

public class List
{
    public class Query : IRequest<Result<PagedList<RecipeDto>>>
    {
        public RecipeParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<RecipeDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<Handler> _logger;

        public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor, ILogger<Handler> logger)
        {
            _userAccessor = userAccessor;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PagedList<RecipeDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _userAccessor.GetUserId();
                _logger.LogInformation($"Fetching recipes for user {currentUserId}");
                _logger.LogInformation($"Page Number: {request.Params.PageNumber}, Page Size: {request.Params.PageSize}");

                // Step 1️⃣: Start query with user filter
                var query = _context.Recipes
                    .Where(r => r.AppUserId == currentUserId);

                // Count total recipes before pagination
                var totalRecipeCount = await query.CountAsync(cancellationToken);
                _logger.LogInformation($"Total recipes found for user: {totalRecipeCount}");

                // Then apply sorting
                if (request.Params?.PriceHigh == true)
                {
                    query = query.OrderByDescending(r => r.Measurements.Any()
                        ? r.Measurements.Sum(m =>
                            m.Amount * (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage))
                        : 0);
                }
                else if (request.Params?.PriceLow == true)
                {
                    query = query.OrderBy(r => r.Measurements.Any()
                        ? r.Measurements.Sum(m =>
                            m.Amount * (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage))
                        : 0);
                }
                else
                {
                    query = query.OrderBy(r => r.Name);
                }

                // Apply category filter
                if (request.Params?.RecipeCategory.HasValue == true)
                {
                    query = query.Where(r => r.RecipeCategory == request.Params.RecipeCategory);
                }

                // Fetch the data with all necessary includes
                var recipes = await query
                    .Include(r => r.Measurements)
                    .ThenInclude(m => m.Ingredient)
                    .ThenInclude(i => i.Nutrition)
                    .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                    .Take(request.Params.PageSize)
                    .ToListAsync(cancellationToken);

                // Detailed logging for each recipe
                foreach (var recipe in recipes)
                {
                    _logger.LogInformation($"Recipe: {recipe.Name}");
                    _logger.LogInformation($"  Servings: {recipe.ServingsPerRecipe}");
                    _logger.LogInformation($"  Measurements Count: {recipe.Measurements?.Count ?? 0}");

                    if (recipe.Measurements != null)
                    {
                        foreach (var measurement in recipe.Measurements)
                        {
                            _logger.LogInformation($"    Ingredient: {measurement.Ingredient.Name}");
                            _logger.LogInformation($"    Amount: {measurement.Amount}");
                            _logger.LogInformation($"    Ingredient Nutrition:");
                            _logger.LogInformation($"      Calories: {measurement.Ingredient.Nutrition?.Calories}");
                            _logger.LogInformation($"      Carbs: {measurement.Ingredient.Nutrition?.Carbs}");
                            _logger.LogInformation($"      Fat: {measurement.Ingredient.Nutrition?.Fat}");
                            _logger.LogInformation($"      Protein: {measurement.Ingredient.Nutrition?.Protein}");
                        }
                    }
                }

                // Manual mapping with calculations
                var recipeDtos = recipes.Select(r => new RecipeDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    RecipeCategory = r.RecipeCategory.ToString(),
                    ServingsPerRecipe = r.ServingsPerRecipe,
                    AppUserId = r.AppUserId,
                    Instructions = r.Instructions,

                    TotalPrice = r.Measurements?.Sum(m =>
                        m.Amount * (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage)) ?? 0m,

                    PricePerServing = r.ServingsPerRecipe > 0
                        ? (r.Measurements?.Sum(m =>
                               m.Amount * (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage))
                           / r.ServingsPerRecipe) ?? 0m
                        : 0m,

                    CaloriesPerRecipe = r.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Calories) ?? 0,

                    CarbsPerRecipe = r.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Carbs) ?? 0,

                    FatPerRecipe = r.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Fat) ?? 0,

                    ProteinPerRecipe = r.Measurements?.Sum(m =>
                        (double)m.Amount * m.Ingredient.Nutrition.Protein) ?? 0,

                    Nutrition = new RecipeNutritionDto
                    {
                        RecipeId = r.Id,
                        TotalCalories = r.Measurements?.Sum(m =>
                            (double)m.Amount * m.Ingredient.Nutrition.Calories) ?? 0,
                        TotalCarbs = r.Measurements?.Sum(m =>
                            (double)m.Amount * m.Ingredient.Nutrition.Carbs) ?? 0,
                        TotalFat = r.Measurements?.Sum(m =>
                            (double)m.Amount * m.Ingredient.Nutrition.Fat) ?? 0,
                        TotalProtein = r.Measurements?.Sum(m =>
                            (double)m.Amount * m.Ingredient.Nutrition.Protein) ?? 0,
                        CaloriesPerServing = r.ServingsPerRecipe > 0
                            ? (r.Measurements?.Sum(m =>
                                (double)m.Amount * m.Ingredient.Nutrition.Calories) / r.ServingsPerRecipe) ?? 0
                            : 0,
                        CarbsPerServing = r.ServingsPerRecipe > 0
                            ? (r.Measurements?.Sum(m =>
                                (double)m.Amount * m.Ingredient.Nutrition.Carbs) / r.ServingsPerRecipe) ?? 0
                            : 0,
                        FatPerServing = r.ServingsPerRecipe > 0
                            ? (r.Measurements?.Sum(m =>
                                (double)m.Amount * m.Ingredient.Nutrition.Fat) / r.ServingsPerRecipe) ?? 0
                            : 0,
                        ProteinPerServing = r.ServingsPerRecipe > 0
                            ? (r.Measurements?.Sum(m =>
                                (double)m.Amount * m.Ingredient.Nutrition.Protein) / r.ServingsPerRecipe) ?? 0
                            : 0
                    }
                }).ToList();

                // Logging
                _logger.LogInformation($"Fetched {recipeDtos.Count} recipes");
                foreach (var recipe in recipeDtos)
                {
                    _logger.LogDebug($"Recipe: {recipe.Name}, Total Calories: {recipe.Nutrition?.TotalCalories}, Price Per Serving: {recipe.PricePerServing}");
                }

                // Create PagedList manually
                var totalCount = await query.CountAsync(cancellationToken);

                return Result<PagedList<RecipeDto>>.Success(
                    new PagedList<RecipeDto>(
                        recipeDtos,
                        totalCount,
                        request.Params.PageNumber,
                        request.Params.PageSize
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching recipes");
                return Result<PagedList<RecipeDto>>.Failure("An error occurred while fetching recipes");
            }
        }
    }
}


/*
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Core;
using Application.Interfaces;
using AutoMapper.QueryableExtensions;


namespace Application.Recipes;

public class List
{
public class Query : IRequest<Result<PagedList<RecipeDto>>>
{
public RecipeParams Params { get; set; }
}

public class Handler : IRequestHandler<Query, Result<PagedList<RecipeDto>>>
{
private readonly DataContext _context;
private readonly IMapper _mapper;
private readonly IUserAccessor _userAccessor;

public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
{
    _userAccessor = userAccessor;
    _context = context;
    _mapper = mapper;
}

public async Task<Result<PagedList<RecipeDto>>> Handle(Query request, CancellationToken cancellationToken)
{
    var currentUserId = _userAccessor.GetUserId();

    // Step 1️⃣: Start query with user filter
    var query = _context.Recipes
        .Where(r => r.AppUserId == currentUserId)
        .OrderBy(r => r.Name);
        //.AsQueryable();


    // Step 2️⃣: Apply Sorting Filters
    if (request.Params?.PriceHigh == true)
    {
        query = query.OrderByDescending(r => r.Measurements.Sum(m => m.Amount *
                                            (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage)));
    }

    if (request.Params?.PriceLow == true)
    {
        query = query.OrderBy(r => r.Measurements.Sum(m => m.Amount *
                                     (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage)));
    }

    if (request.Params?.RecipeCategory.HasValue == true)
    {
        query = query.Where(r => r.RecipeCategory == request.Params.RecipeCategory);
    }

    // Step 3️⃣: Use AutoMapper's `ProjectTo<>` to retrieve mapped data
    var projectedQuery = query
        .ProjectTo<RecipeDto>(_mapper.ConfigurationProvider)
        .AsQueryable();

    // Step 4️⃣: Convert projected data to list for additional computations
    var recipesDto = await projectedQuery.ToListAsync(cancellationToken);

    // Step 5️⃣: Attach Computed Fields Separately (EF Core doesn't allow them inside `ProjectTo<>`)
    foreach (var recipeDto in recipesDto)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Measurements)
            .ThenInclude(m => m.Ingredient)
            .ThenInclude(i => i.Nutrition)
            .FirstOrDefaultAsync(r => r.Id == recipeDto.Id, cancellationToken);

        if (recipe != null)
        {
            recipeDto.TotalPrice = recipe.Measurements?.Sum(m => m.Amount *
                                                (m.Ingredient.PricePerPackage / m.Ingredient.MeasurementsPerPackage)) ?? 0;

            recipeDto.PricePerServing = recipe.ServingsPerRecipe > 0
                ? recipeDto.TotalPrice / recipe.ServingsPerRecipe
                : 0;

            recipeDto.CaloriesPerRecipe = recipe.Measurements?.Sum(m => (double)m.Amount * m.Ingredient.Nutrition.Calories) ?? 0;
            recipeDto.CarbsPerRecipe = recipe.Measurements?.Sum(m => (double)m.Amount * m.Ingredient.Nutrition.Carbs) ?? 0;
            recipeDto.FatPerRecipe = recipe.Measurements?.Sum(m => (double)m.Amount * m.Ingredient.Nutrition.Fat) ?? 0;
            recipeDto.ProteinPerRecipe = recipe.Measurements?.Sum(m => (double)m.Amount * m.Ingredient.Nutrition.Protein) ?? 0;
        }
    }

    // Step 6️⃣: Paginate the final list after calculations
    return Result<PagedList<RecipeDto>>.Success(
        await PagedList<RecipeDto>.CreateAsync(recipesDto.AsQueryable(), request.Params.PageNumber,
            request.Params.PageSize)
    );
}
}
}
*/
