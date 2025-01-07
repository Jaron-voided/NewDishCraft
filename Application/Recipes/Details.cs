using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Recipes;

public class Details
{
    public class Query : IRequest<Recipe>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Recipe>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Recipe> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _context.Recipes
                .Include(r => r.Measurements)
                .FirstOrDefaultAsync(r => r.Id == request.Id);
        }
    }
}