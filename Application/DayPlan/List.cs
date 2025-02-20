

using Application.Core;
using Application.DayPlan.DTOs;
using Application.DayPlans;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.DayPlan;

public class List
{
    public class Query : IRequest<Result<PagedList<DayPlanDto>>>
    {
        public DayPlanParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<DayPlanDto>>>
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

        public async Task<Result<PagedList<DayPlanDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var currentUserId = _userAccessor.GetUserId();

            var query = _context.DayPlans
                .Where(dp => dp.AppUserId == currentUserId)
                .Include(dp => dp.DayPlanRecipes)
                .ThenInclude(dpr => dpr.Recipe)
                .ThenInclude(r => r.Measurements)
                .ThenInclude(m => m.Ingredient)
                .OrderBy(dp => dp.Name);

            // Add any additional filtering based on DayPlanParams if needed

            var projectedQuery = query
                .ProjectTo<DayPlanDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            return Result<PagedList<DayPlanDto>>.Success(
                await PagedList<DayPlanDto>.CreateAsync(
                    projectedQuery, 
                    request.Params.PageNumber, 
                    request.Params.PageSize
                )
            );
        }
    }
}