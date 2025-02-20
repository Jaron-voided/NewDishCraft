using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ingredients;

public class Details
{
    public class Query : IRequest<Result<IngredientDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<IngredientDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<IngredientDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ingredient = await _context.Ingredients
                .Include(i => i.Nutrition)
                .Include(i => i.MeasurementUnit)
                .ProjectTo<IngredientDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            
            return Result<IngredientDto>.Success(ingredient);
        }
    }
}