using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Ingredients;

public class List
{
    public class Query : IRequest<Result<PagedList<IngredientDto>>>
    {
        public IngredientParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<IngredientDto>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IUserAccessor _userAccessor;

        public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userAccessor = userAccessor;
        }

        public async Task<Result<PagedList<IngredientDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            // ✅ Get the current user's ID
            var currentUserId = _userAccessor.GetUserId();

            // ✅ Start query: Filter by current user
            var query = _context.Ingredients
                .Include(i => i.Nutrition)  // New Line
                .Include(i => i.MeasurementUnit)  // New Line
                .Where(i => i.AppUserId == currentUserId)
                .OrderBy(i => i.Name)
                .AsQueryable();  // 🔹 ProjectTo<> should come later
            
            var projectedQuery = query.ProjectTo<IngredientDto>(_mapper.ConfigurationProvider);


            // ✅ Apply Filters Dynamically
            if (request.Params?.PriceHigh == true)
            {
                projectedQuery = projectedQuery
                    .Where(i => i.MeasurementsPerPackage > 0)
                    .OrderByDescending(i => i.PricePerMeasurement);
                    //.OrderByDescending(i => (double)i.PricePerPackage / (double)i.MeasurementsPerPackage);
            }

            if (request.Params?.PriceLow == true)
            {
                projectedQuery = projectedQuery
                    .Where(i => i.MeasurementsPerPackage > 0)
                    .OrderBy(i => i.PricePerMeasurement);
                    //.OrderBy(i => (double)i.PricePerPackage / (double)i.MeasurementsPerPackage);
            }

            if (request.Params?.Category.HasValue == true)
            {
                projectedQuery = projectedQuery.Where(i => i.Category == request.Params.Category.ToString());
            }


            // ✅ Return Paged List
            return Result<PagedList<IngredientDto>>.Success(
                await PagedList<IngredientDto>.CreateAsync(
                    projectedQuery, request.Params?.PageNumber ?? 1, request.Params?.PageSize ?? 10
                )
            );
        }
    }
}

