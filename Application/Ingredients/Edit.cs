using AutoMapper;
using Domain;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ingredients;

public class Edit
{
    public class Command : IRequest
    {
        public Ingredient Ingredient { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.Nutrition)
                .Include(i => i.MeasurementUnit)
                .FirstOrDefaultAsync(i => i.Id == request.Ingredient.Id);
                //.FindAsync(request.Ingredient.Id);
                
            if (ingredient == null) throw new Exception("Ingredient not found");
            
            _mapper.Map(request.Ingredient, ingredient);

            if (request.Ingredient.Nutrition != null)
            {
                // Ensure Nutrition exists
                if (ingredient.Nutrition == null)
                {
                    // Add new Nutrition if it doesn't exist
                    ingredient.Nutrition = new NutritionalInfo
                    {
                        IngredientId = ingredient.Id,
                        Calories = request.Ingredient.Nutrition.Calories,
                        Carbs = request.Ingredient.Nutrition.Carbs,
                        Fat = request.Ingredient.Nutrition.Fat,
                        Protein = request.Ingredient.Nutrition.Protein
                    };
                    _context.NutritionalInfos.Add(ingredient.Nutrition);
                }
                else
                {
                    // Update existing Nutrition
                    ingredient.Nutrition.Calories = request.Ingredient.Nutrition.Calories;
                    ingredient.Nutrition.Carbs = request.Ingredient.Nutrition.Carbs;
                    ingredient.Nutrition.Fat = request.Ingredient.Nutrition.Fat;
                    ingredient.Nutrition.Protein = request.Ingredient.Nutrition.Protein;
                }
            }
            
            if (request.Ingredient.MeasurementUnit != null)
            {
                if (ingredient.MeasurementUnit == null)
                {
                    // Add new MeasurementUnit if it doesn't exist
                    ingredient.MeasurementUnit = new MeasurementUnit
                    {
                        IngredientId = ingredient.Id,
                        MeasuredIn = request.Ingredient.MeasurementUnit.MeasuredIn,
                        WeightUnit = request.Ingredient.MeasurementUnit.WeightUnit,
                        VolumeUnit = request.Ingredient.MeasurementUnit.VolumeUnit
                    };
                    _context.MeasurementUnits.Add(ingredient.MeasurementUnit);
                }
                else
                {
                    // Update existing MeasurementUnit
                    ingredient.MeasurementUnit.MeasuredIn = request.Ingredient.MeasurementUnit.MeasuredIn;
                    ingredient.MeasurementUnit.WeightUnit = request.Ingredient.MeasurementUnit.WeightUnit;
                    ingredient.MeasurementUnit.VolumeUnit = request.Ingredient.MeasurementUnit.VolumeUnit;
                }
            }

            
            await _context.SaveChangesAsync();
        }
    }
}