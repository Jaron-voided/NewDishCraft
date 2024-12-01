using MediatR;
using Persistence;

namespace Application.Ingredients;

public class Delete
{
    public class Command : IRequest
    {
        public Guid Id { get; set; }
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
            var ingredient = await _context.Ingredients.FindAsync(request.Id);
            
            if (ingredient == null) throw new Exception("Ingredient not found.");
            
            _context.Remove(ingredient);
            
            await _context.SaveChangesAsync();
        }
    }
}