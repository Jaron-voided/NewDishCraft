using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Recipes;

public class Edit
{
    public class Command : IRequest
    {
        public Recipe Recipe { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _context;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMediator mediator, IMapper mapper)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == request.Recipe.Id);

            if (recipe == null) throw new Exception("Recipe not found");

            _mapper.Map(request.Recipe, recipe);

            if (request.Recipe.Measurements != null && request.Recipe.Measurements.Any())
            {
                foreach (var measurement in request.Recipe.Measurements)
                {
                    var existingMeasurement = recipe.Measurements.FirstOrDefault(m
                        => m.IngredientId == measurement.IngredientId);

                    if (existingMeasurement == null)
                    {
                        await _mediator.Send(new Application.Measurements.Create.Command
                        {
                            RecipeId = recipe.Id,
                            IngredientId = measurement.IngredientId,
                            Amount = measurement.Amount
                        });
                    }
                    else
                    {
                        await _mediator.Send(new Application.Measurements.Edit.Command
                        {
                            RecipeId = recipe.Id,
                            IngredientId = measurement.IngredientId,
                            Amount = measurement.Amount
                        });
                    }
                }
            }
            
            await _context.SaveChangesAsync();
        }
    }
}