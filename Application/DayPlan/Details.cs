using Application.Core;
using Application.DayPlan.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.DayPlan;

public class Details
{
    public class Query : IRequest<Result<DayPlanDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<DayPlanDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        private readonly IMediator _mediator; 

        public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger, IMediator mediator)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Result<DayPlanDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 Fetching DayPlan with ID: {Id}", request.Id);

            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("❌ Invalid DayPlan ID received: {Id}", request.Id);
                return Result<DayPlanDto>.Failure("Invalid DayPlan ID");
            }

            var dayPlan = await _context.DayPlans
                .Where(d => d.Id == request.Id)
                .Include(d => d.DayPlanRecipes)
                    .ThenInclude(dpr => dpr.Recipe)
                        .ThenInclude(r => r.Measurements)
                            .ThenInclude(m => m.Ingredient)
                .ProjectTo<DayPlanDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (dayPlan == null)
            {
                _logger.LogWarning("❌ DayPlan with ID {Id} not found.", request.Id);
                return Result<DayPlanDto>.Failure("DayPlan not found");
            }
            
            // Calculate totals
            var calculationsResult = await _mediator.Send(new CalculateDayPlanTotals.Query 
            { 
                DayPlanId = dayPlan.Id 
            }, cancellationToken);

            if (calculationsResult.IsSuccess)
            {
                dayPlan.PriceForDay = calculationsResult.Value.TotalPrice;
                dayPlan.CaloriesPerDay = (decimal)calculationsResult.Value.TotalCalories;
                dayPlan.CarbsPerDay = (decimal)calculationsResult.Value.TotalCarbs;
                dayPlan.FatPerDay = (decimal)calculationsResult.Value.TotalFat;
                dayPlan.ProteinPerDay = (decimal)calculationsResult.Value.TotalProtein;
            }


            _logger.LogInformation("✅ Successfully retrieved DayPlan with ID: {Id}", dayPlan.Id);
            return Result<DayPlanDto>.Success(dayPlan);
        }
    }
}


/*using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.DayPlan;

public class Details
{
    public class Query : IRequest<Result<DayPlanDto>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<DayPlanDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<DayPlanDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            
            if (request.Id == Guid.Empty)
            {
                Console.WriteLine("❌ Invalid DayPlan ID received: {DayPlanId}", request.Id);
                return Result<DayPlanDto>.Failure("Invalid DayPlan ID");
            }
            
            var dayPlan = await _context.DayPlans
                .Where(d => d.Id == request.Id) // ✅ Filter first
                .Include(d => d.DayPlanRecipes) // ✅ Load Measurements
                    .ThenInclude(dpr => dpr.Recipe) // ✅ Load Ingredients
                        .ThenInclude(r => r.Measurements) // ✅ Load Nutrition
                            .ThenInclude(m => m.Ingredient) // ✅ Load Nutrition
                .ProjectTo<DayPlanDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (dayPlan == null)
            {
                Console.WriteLine("❌ DayPlan with ID {0} not found.", request.Id);
                Result<DayPlanDto>.Failure("DayPlan not found");
                //return Result<DayPlanDto>.Failure("DayPlan not found");
            }
            return Result<DayPlanDto>.Success(dayPlan);
        }
    }
}*/