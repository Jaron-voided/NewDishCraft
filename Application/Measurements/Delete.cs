using MediatR;
using Persistence;

namespace Application.Measurements;

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
            var measurement = await _context.Measurements.FindAsync(request.Id);
            
            if (measurement == null) throw new Exception("Measurement not found.");
            
            _context.Remove(measurement);
            
            await _context.SaveChangesAsync();
        }
    }
}