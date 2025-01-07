using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Measurements;

public class ListByRecipe
{
    public class Query : IRequest<List<Measurement>>
    {
        public Guid RecipeId { get; set; }

        /*
        public Query(Guid recipeId)
        {
            RecipeId = recipeId;
        }
        */
        
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
                .Where(m => m.RecipeId == request.RecipeId)
                .Include(m => m.Ingredient)
                .Include(m => m.Recipe)
                .ToListAsync();
        }
    }
}