using Application.Core;
using Application.DayPlan.DTOs;
using Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.DayPlan;

public class CalculateDayPlanTotals
{
    public class Query : IRequest<Result<DayPlanCalculationsDto>>
    {
        public Guid DayPlanId { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<DayPlanCalculationsDto>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;
        //private readonly IngredientService _ingredientService;

        public Handler(DataContext context, ILogger<Handler> logger, IngredientService ingredientService)
        {
            _context = context;
            _logger = logger;
           // _ingredientService = ingredientService;
        }

        public async Task<Result<DayPlanCalculationsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 Calculating totals for DayPlan: {Id}", request.DayPlanId);

            var dayPlan = await _context.DayPlans
                .Include(d => d.DayPlanRecipes)
                    .ThenInclude(dpr => dpr.Recipe)
                        .ThenInclude(r => r.Measurements)
                            .ThenInclude(m => m.Ingredient)
                                .ThenInclude(i => i.Nutrition)
                .FirstOrDefaultAsync(d => d.Id == request.DayPlanId, cancellationToken);

            if (dayPlan == null)
            {
                _logger.LogWarning("❌ DayPlan not found: {Id}", request.DayPlanId);
                return Result<DayPlanCalculationsDto>.Failure("DayPlan not found");
            }

            var calculations = new DayPlanCalculationsDto();

            foreach (var dayPlanRecipe in dayPlan.DayPlanRecipes)
            {
                var scalingFactor = (double)dayPlanRecipe.Servings / dayPlanRecipe.Recipe.ServingsPerRecipe;

                foreach (var measurement in dayPlanRecipe.Recipe.Measurements)
                {
                    var ingredient = measurement.Ingredient;
                    var measurementAmount = (double)measurement.Amount;
                    
                    // Calculate price
                    var pricePerMeasurement = ingredient.PricePerPackage / ingredient.MeasurementsPerPackage;
                    calculations.TotalPrice += pricePerMeasurement * (decimal)measurementAmount * (decimal)scalingFactor;

                    // Calculate nutrition
                    if (ingredient.Nutrition != null)
                    {
                        calculations.TotalCalories += ingredient.Nutrition.Calories * measurementAmount * scalingFactor;
                        calculations.TotalCarbs += ingredient.Nutrition.Carbs * measurementAmount * scalingFactor;
                        calculations.TotalFat += ingredient.Nutrition.Fat * measurementAmount * scalingFactor;
                        calculations.TotalProtein += ingredient.Nutrition.Protein * measurementAmount * scalingFactor;
                    }
                }
            }

            _logger.LogInformation("✅ Successfully calculated totals for DayPlan: {Id}", request.DayPlanId);
            return Result<DayPlanCalculationsDto>.Success(calculations);
        }
    }
}