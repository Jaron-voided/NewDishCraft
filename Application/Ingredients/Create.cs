using Domain;
using Domain.Models;
using MediatR;
using Persistence;

namespace Application.Ingredients;

public class Create
{
    public class Command : IRequest
    {
        public Ingredient Ingredient { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }
        
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            // Step 1: Add the Ingredient and save to generate its ID
            _context.Ingredients.Add(request.Ingredient);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 2: Handle NutritionalInfo
            if (request.Ingredient.Nutrition != null)
            {
                request.Ingredient.Nutrition.IngredientId = request.Ingredient.Id;
                _context.NutritionalInfos.Add(request.Ingredient.Nutrition);
            }

            // Step 3: Handle MeasurementUnit
            if (request.Ingredient.MeasurementUnit != null)
            {
                request.Ingredient.MeasurementUnit.IngredientId = request.Ingredient.Id;
                _context.MeasurementUnits.Add(request.Ingredient.MeasurementUnit);
            }

            // Step 4: Save changes to the related entities
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}