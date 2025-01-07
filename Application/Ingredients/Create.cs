using Application.Core;
using Domain.Models;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Ingredients;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public Ingredient Ingredient { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Ingredient).SetValidator(new IngredientValidator());
            
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Step 1: Add the Ingredient and save to generate its ID
            _context.Ingredients.Add(request.Ingredient);
            //await _context.SaveChangesAsync(cancellationToken); // Generate Ingredient.Id here

            // Step 2: Handle MeasurementUnit
            if (request.Ingredient.MeasurementUnit != null)
            {
                if (request.Ingredient.MeasurementUnit.IngredientId == null)
                {
                    request.Ingredient.MeasurementUnit.IngredientId = request.Ingredient.Id;
                }

                _context.MeasurementUnits.Add(request.Ingredient.MeasurementUnit);
            }

            // Step 3: Handle NutritionalInfo
            if (request.Ingredient.Nutrition != null)
            {
                if (request.Ingredient.Nutrition.IngredientId == null)
                {
                    request.Ingredient.Nutrition.IngredientId = request.Ingredient.Id;
                }

                _context.NutritionalInfos.Add(request.Ingredient.Nutrition);
            }

            // Step 4: Save changes to the related entities
            /*
            await _context.SaveChangesAsync(cancellationToken);
            */
            var result = await _context.SaveChangesAsync() > 0;
            if (!result) return Result<Unit>.Failure("Failed to create ingredient");
            
            return Result<Unit>.Success(Unit.Value);
        }
    }
}