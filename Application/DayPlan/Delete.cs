using Application.Core;
using MediatR;
using Persistence;

namespace Application.DayPlan;

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
            var dayPlan = await _context.DayPlans.FindAsync(request.Id);

            if (dayPlan == null) 
            {
                Console.WriteLine($"❌ DayPlan with ID {request.Id} not found before deletion."); 
                return Result<Unit>.Failure("Not Found"); // ✅ Return Not Found
            }

            Console.WriteLine($"🛠️ Deleting DayPlan with ID {request.Id}...");
            _context.DayPlans.Remove(dayPlan);

            var result = await _context.SaveChangesAsync() > 0;
            if (!result) 
            {
                Console.WriteLine($"⚠️ Failed to delete dayPlan with ID {request.Id}.");
                return Result<Unit>.Failure("Failed to delete dayPlan");
            }

            Console.WriteLine($"✅ DayPlan with ID {request.Id} successfully deleted.");
            return Result<Unit>.Success(Unit.Value); // ✅ Return success if deleted
        }
    }
}