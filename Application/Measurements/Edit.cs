using AutoMapper;
using Domain;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Measurements;

public class Edit
{
    public class Command : IRequest
    {
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
        public decimal Amount { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var measurement = await _context.Measurements
                .FirstOrDefaultAsync(m =>
                    m.RecipeId == request.RecipeId && m.IngredientId == request.IngredientId);

            if (measurement == null)
            {
                throw new Exception("Measurement not found");
            }

            // Update the amount
            measurement.Amount = request.Amount;

            // Save changes
            await _context.SaveChangesAsync();
        }
    }
}