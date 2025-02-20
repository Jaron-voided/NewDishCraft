using Application.Core;
using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ingredients;

public class Edit
{
    public class Command : IRequest<Result<Unit>>
    {
        public IngredientDto IngredientDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.IngredientDto).SetValidator(new IngredientValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ingredient dto name: {request.IngredientDto.Name}");

            var ingredient = await _context.Ingredients
                .Include(i => i.Nutrition)
                .Include(i => i.MeasurementUnit) // ✅ Ensure MeasurementUnit is loaded
                .FirstOrDefaultAsync(i => i.Id == request.IngredientDto.Id);

            if (ingredient == null)
            {
                Console.WriteLine($"Ingredient with ID {request.IngredientDto.Id} not found.");
                return Result<Unit>.Failure("Ingredient not found");
            }

            Console.WriteLine($"Found ingredient: {ingredient.Name}, ID: {ingredient.Id}");

            // ✅ Map DTO properties to Ingredient (excluding MeasurementUnit)
            _mapper.Map(request.IngredientDto, ingredient);
            Console.WriteLine($"Mapped Ingredient: {ingredient.Id}, AppUserId: {ingredient.AppUserId}");

            // ✅ Manually update MeasurementUnit instead of letting AutoMapper overwrite it
            if (ingredient.MeasurementUnit != null)
            {
                Console.WriteLine($"Updating existing MeasurementUnit for {ingredient.Name}");
                ingredient.MeasurementUnit.MeasuredIn = Enum.Parse<MeasuredIn>(request.IngredientDto.MeasuredIn);
                _context.Entry(ingredient.MeasurementUnit).Property(m => m.MeasuredIn).IsModified = true;
            }

            // ✅ Handle Nutrition updates
            if (ingredient.Nutrition == null)
            {
                Console.WriteLine($"Nutrition was NULL for {ingredient.Name}. Creating new one...");
                ingredient.Nutrition = new NutritionalInfo
                {
                    IngredientId = ingredient.Id,
                    Calories = request.IngredientDto.Nutrition.Calories,
                    Carbs = request.IngredientDto.Nutrition.Carbs,
                    Fat = request.IngredientDto.Nutrition.Fat,
                    Protein = request.IngredientDto.Nutrition.Protein
                };
                _context.NutritionalInfos.Add(ingredient.Nutrition);
            }
            else
            {
                Console.WriteLine($"Updating existing Nutrition for {ingredient.Name}");
                ingredient.Nutrition.Calories = request.IngredientDto.Nutrition.Calories;
                ingredient.Nutrition.Carbs = request.IngredientDto.Nutrition.Carbs;
                ingredient.Nutrition.Fat = request.IngredientDto.Nutrition.Fat;
                ingredient.Nutrition.Protein = request.IngredientDto.Nutrition.Protein;

                _context.Entry(ingredient.Nutrition).Property(n => n.Calories).IsModified = true;
                _context.Entry(ingredient.Nutrition).Property(n => n.Carbs).IsModified = true;
                _context.Entry(ingredient.Nutrition).Property(n => n.Fat).IsModified = true;
                _context.Entry(ingredient.Nutrition).Property(n => n.Protein).IsModified = true;
            }

            var result = await _context.SaveChangesAsync() > 0;

            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to update ingredient.");
        }
    }
}