using Domain;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ingredients;

public class Details
{
    public class Query : IRequest<Ingredient>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Ingredient>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }
        public async Task<Ingredient> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Ingredients
                .Include(i => i.Nutrition)
                .Include(i => i.MeasurementUnit)
                .FirstOrDefaultAsync(i => i.Id == request.Id);
            //.FindAsync(request.Id);
        }
    }
}