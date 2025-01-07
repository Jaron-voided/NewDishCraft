
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Measurements;

public class List
{
    public class Query : IRequest<List<Measurement>>
    {
    }

    public class Handler : IRequestHandler<Query, List<Measurement>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Measurement>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Measurements
                .Include(m => m.Ingredient)
                .Include(m => m.Recipe)
                .ToListAsync();
        }
    }
}
