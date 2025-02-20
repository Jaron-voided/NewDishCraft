using Application.Core;
using MediatR;
using Persistence;

namespace Application.Recipes;

public class Delete
{
    public class Command : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
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
            var recipe = await _context.Recipes.FindAsync(request.Id);

            if (recipe == null) 
            {
                Console.WriteLine($"❌ Recipe with ID {request.Id} not found before deletion."); 
                return Result<Unit>.Failure("Not Found"); // ✅ Return Not Found
            }

            Console.WriteLine($"🛠️ Deleting recipe with ID {request.Id}...");
            _context.Recipes.Remove(recipe);

            var result = await _context.SaveChangesAsync() > 0;
            if (!result) 
            {
                Console.WriteLine($"⚠️ Failed to delete recipe with ID {request.Id}.");
                return Result<Unit>.Failure("Failed to delete recipe");
            }

            Console.WriteLine($"✅ Recipe with ID {request.Id} successfully deleted.");
            return Result<Unit>.Success(Unit.Value); // ✅ Return success if deleted
        }
    }
}