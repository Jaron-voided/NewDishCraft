using Domain;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Ingredients;

public class List
{
    public class Query : IRequest<List<Ingredient>>
    {
    }

    public class Handler : IRequestHandler<Query, List<Ingredient>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }
        
        public async Task<List<Ingredient>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Ingredients
                .Include(i => i.Nutrition)
                .Include(i => i.MeasurementUnit)
                .ToListAsync();
        }
    }
}