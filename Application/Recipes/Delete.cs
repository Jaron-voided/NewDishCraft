using MediatR;
using Persistence;

namespace Application.Recipes;

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
            var recipe = await _context.Recipes.FindAsync(request.Id);
            
            if (recipe == null) throw new Exception("Recipe not found.");
            
            _context.Remove(recipe);
            
            await _context.SaveChangesAsync();
        }
    }
}