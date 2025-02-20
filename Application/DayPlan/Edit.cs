using Application.Core;
using Application.DayPlan;
using Application.DayPlan.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.DayPlan;

public class Edit
{
    public class Command : IRequest<Result<Unit>>
    {
        //public Guid Id { get; set; }
        public DayPlanDto DayPlanDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.DayPlanDto).SetValidator(new DayPlanValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IUserAccessor userAccessor, ILogger<Handler> logger, IMapper mapper)
        {
            _context = context;
            _userAccessor = userAccessor;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"🔍 Fetching DayPlan with ID: {request.DayPlanDto.Id}");
            Console.WriteLine($"🔍 Fetching DayPlan with ID: {request.DayPlanDto.Id}");

            var dayPlan = await _context.DayPlans
                .FirstOrDefaultAsync(r => r.Id == request.DayPlanDto.Id, cancellationToken);

            if (dayPlan == null)
            {
                _logger.LogWarning($"❌ DayPlan with ID {request.DayPlanDto.Id} not found.");
                Console.WriteLine($"❌ DayPlan with ID {request.DayPlanDto.Id} not found.");
                return Result<Unit>.Failure("DayPlan not found");
            }

            var username = _userAccessor.GetUsername();
            _logger.LogInformation($"✅ Retrieved username: {username}");
            Console.WriteLine($"✅ Retrieved username: {username}");

            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("❌ Unable to retrieve the logged-in user.");
                return Result<Unit>.Failure("Unauthorized");
            }

            // **Ensure the user owns the dayPlan**
            if (dayPlan.AppUserId != request.DayPlanDto.AppUserId)
            {
                _logger.LogWarning($"❌ Unauthorized user {username} attempted to edit dayPlan {dayPlan.Id}");
                return Result<Unit>.Failure("Unauthorized");
            }

            // **Map changes from DTO**
            _mapper.Map(request.DayPlanDto, dayPlan);

            _logger.LogInformation($"🛠️ Updating dayPlan '{dayPlan.Name}'...");
            Console.WriteLine($"🛠️ Updating dayPlan '{dayPlan.Name}'...");

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                _logger.LogError($"❌ Failed to update dayPlan '{dayPlan.Name}'.");
                Console.WriteLine($"❌ Failed to update dayPlan '{dayPlan.Name}'.");
                return Result<Unit>.Failure("Failed to update dayPlan.");
            }

            _logger.LogInformation($"✅ DayPlan '{dayPlan.Name}' successfully updated.");
            Console.WriteLine($"✅ DayPlan '{dayPlan.Name}' successfully updated.");

            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to update dayPlan.");
            /*
            return Result<Unit>.Success(_mapper.Map(request.DayPlanDto, dayPlan));
        */
        }
    }
}
