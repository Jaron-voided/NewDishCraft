using Application.Core;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Ingredients;

public class NutritionDetails
{
    public class Query : IRequest<Result<NutritionalInfoDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<NutritionalInfoDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<NutritionalInfoDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var nutrition = await _context.NutritionalInfos
                .FirstOrDefaultAsync(n => n.IngredientId == request.Id);

            if (nutrition == null)
            {
                // Create default nutrition if not exists
                nutrition = new NutritionalInfo 
                { 
                    IngredientId = request.Id, 
                    Calories = 0, 
                    Carbs = 0, 
                    Fat = 0, 
                    Protein = 0 
                };
                _context.NutritionalInfos.Add(nutrition);
                await _context.SaveChangesAsync();
            }

            return Result<NutritionalInfoDto>.Success(_mapper.Map<NutritionalInfoDto>(nutrition));
        }
    }
}