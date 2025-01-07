using Domain;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Measurements;

public class Details
{
    public class Query : IRequest<Measurement>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Measurement>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }
        public async Task<Measurement> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Measurements
                .Include(m => m.Ingredient)
                .Include(m => m.Recipe)
                .FirstOrDefaultAsync(i => i.Id == request.Id);
            
        }
    }
}