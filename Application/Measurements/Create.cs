using Domain.Models;
using MediatR;
using Persistence;

namespace Application.Measurements;

public class Create
{
    public class Command : IRequest
    {
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
        public decimal Amount { get; set; }
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
            // Step 1: Create a new Measurement
            var measurement = new Measurement
            {
                Id = Guid.NewGuid(),
                RecipeId = request.RecipeId,
                IngredientId = request.IngredientId,
                Amount = request.Amount
            };

            // Step 2: Add the Measurement to the database
            _context.Measurements.Add(measurement);

            // Step 3: Save changes to the database
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}