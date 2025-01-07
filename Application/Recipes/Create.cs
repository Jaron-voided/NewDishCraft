using Domain.Models;
using MediatR;
using Persistence;

namespace Application.Recipes;

public class Create
{
    public class Command : IRequest
    {
        public Recipe Recipe { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _context;
        private readonly IMediator _mediator;

        public Handler(DataContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            // Step 1: Add the Recipe to the database
            _context.Recipes.Add(request.Recipe);

            // Step 2: Save changes to generate the Recipe ID
            await _context.SaveChangesAsync(cancellationToken);

            // Step 3: Handle Measurements
            if (request.Recipe.Measurements != null && request.Recipe.Measurements.Any())
            {
                foreach (var measurement in request.Recipe.Measurements)
                {
                    // Dispatch the Create.Command for each Measurement
                    await _mediator.Send(new Application.Measurements.Create.Command
                    {
                        RecipeId = request.Recipe.Id,
                        IngredientId = measurement.IngredientId,
                        Amount = measurement.Amount
                    });
                }
            }
        }
    }
}