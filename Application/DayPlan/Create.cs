using Application.Core;
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

public class Create
{
    public class Command : IRequest<Result<DayPlanDto>>
    {
        public DayPlanDto DayPlanDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.DayPlanDto).SetValidator(new DayPlanValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<DayPlanDto>>
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

        public async Task<Result<DayPlanDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 Fetching the logged-in user...");

            var username = _userAccessor.GetUsername();
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("❌ Unable to retrieve logged-in user.");
                return Result<DayPlanDto>.Failure("Unable to retrieve the logged-in user.");
            }

            var user = await _context.Users
                .Include(u => u.DayPlans)
                .FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"❌ User not found for username: {username}");
                return Result<DayPlanDto>.Failure("User not found.");
            }

            // ✅ Convert DTO to Entity
            var dayPlan = _mapper.Map<Domain.Models.DayPlan>(request.DayPlanDto);
            dayPlan.Id = Guid.NewGuid();
            dayPlan.AppUser = user;
            dayPlan.AppUserId = user.Id;
            dayPlan.DayPlanRecipes = new List<Domain.Models.DayPlanRecipe>();

            _logger.LogInformation($"🛠️ Adding DayPlan '{dayPlan.Name}' for user {user.Id} with ID {dayPlan.Id}...");
            
            // 🔍 Log AppUserId and RecipeIds
            _logger.LogInformation($"AppUserId: {dayPlan.AppUserId}");
            foreach (var recipe in request.DayPlanDto.DayPlanRecipes)
            {
                _logger.LogInformation($"RecipeId: {recipe.RecipeId}");
            }

            // ✅ Handle DayPlanRecipes if provided
            if (request.DayPlanDto.DayPlanRecipes != null && request.DayPlanDto.DayPlanRecipes.Any())
            {
                foreach (var dayPlanRecipeDto in request.DayPlanDto.DayPlanRecipes)
                {
                    // Ensure the referenced recipe exists
                    var recipe = await _context.Recipes.FindAsync(dayPlanRecipeDto.RecipeId);
                    if (recipe == null)
                    {
                        _logger.LogWarning($"⚠️ Recipe with ID {dayPlanRecipeDto.RecipeId} not found, skipping...");
                        continue;
                    }

                    // Create and add DayPlanRecipe
                    var dayPlanRecipe = new Domain.Models.DayPlanRecipe
                    {
                        Id = Guid.NewGuid(),
                        DayPlanId = dayPlan.Id,
                        RecipeId = recipe.Id,
                        Servings = dayPlanRecipeDto.Servings
                    };

                    _context.DayPlanRecipes.Add(dayPlanRecipe);
                    dayPlan.DayPlanRecipes.Add(dayPlanRecipe);
                }
            }

            _context.DayPlans.Add(dayPlan);

            try
            {
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    _logger.LogError("❌ Failed to save DayPlan to the database.");
                    return Result<DayPlanDto>.Failure("Failed to create DayPlan.");
                }

                _logger.LogInformation($"✅ DayPlan '{dayPlan.Name}' successfully created with ID {dayPlan.Id}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ An error occurred while saving the DayPlan.");
                return Result<DayPlanDto>.Failure("An error occurred while saving the DayPlan.");
            }

            var dayPlanDto = _mapper.Map<DayPlanDto>(dayPlan);
            return Result<DayPlanDto>.Success(dayPlanDto);
        }
    }
}


/*
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.DayPlan;

public class Create
{
    public class Command : IRequest<Result<DayPlanDto>>
    {
        public DayPlanDto DayPlanDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.DayPlanDto).SetValidator(new DayPlanValidator());
        }
    }

   public async Task<Result<DayPlanDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Fetching the logged-in user...");

        var username = _userAccessor.GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("❌ Unable to retrieve logged-in user.");
            return Result<DayPlanDto>.Failure("Unable to retrieve the logged-in user.");
        }

        var user = await _context.Users
            .Include(u => u.DayPlans)
            .FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning($"❌ User not found for username: {username}");
            return Result<DayPlanDto>.Failure("User not found.");
        }

        // ✅ Convert DTO to Entity
        var dayPlan = _mapper.Map<Domain.Models.DayPlan>(request.DayPlanDto);
        dayPlan.Id = Guid.NewGuid();
        dayPlan.AppUser = user;
        dayPlan.AppUserId = user.Id;
        dayPlan.DayPlanRecipes = new List<Domain.Models.DayPlanRecipe>();

        _logger.LogInformation($"🛠️ Adding DayPlan '{dayPlan.Name}' for user {user.Id} with ID {dayPlan.Id}...");
        
        // 🔍 Log AppUserId and RecipeIds
        _logger.LogInformation($"AppUserId: {dayPlan.AppUserId}");
        foreach (var recipe in request.DayPlanDto.DayPlanRecipes)
        {
            _logger.LogInformation($"RecipeId: {recipe.RecipeId}");
        }

        // ✅ Handle DayPlanRecipes if provided
        if (request.DayPlanDto.DayPlanRecipes != null && request.DayPlanDto.DayPlanRecipes.Any())
        {
            foreach (var dayPlanRecipeDto in request.DayPlanDto.DayPlanRecipes)
            {
                // Ensure the referenced recipe exists
                var recipe = await _context.Recipes.FindAsync(dayPlanRecipeDto.RecipeId);
                if (recipe == null)
                {
                    _logger.LogWarning($"⚠️ Recipe with ID {dayPlanRecipeDto.RecipeId} not found, skipping...");
                    continue;
                }

                // Create and add DayPlanRecipe
                var dayPlanRecipe = new Domain.Models.DayPlanRecipe
                {
                    Id = Guid.NewGuid(),
                    DayPlanId = dayPlan.Id,
                    RecipeId = recipe.Id,
                    Servings = dayPlanRecipeDto.Servings
                };

                _context.DayPlanRecipes.Add(dayPlanRecipe);
                dayPlan.DayPlanRecipes.Add(dayPlanRecipe);
            }
        }

        _context.DayPlans.Add(dayPlan);

        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                _logger.LogError("❌ Failed to save DayPlan to the database.");
                return Result<DayPlanDto>.Failure("Failed to create DayPlan.");
            }

            _logger.LogInformation($"✅ DayPlan '{dayPlan.Name}' successfully created with ID {dayPlan.Id}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ An error occurred while saving the DayPlan.");
            return Result<DayPlanDto>.Failure("An error occurred while saving the DayPlan.");
        }

        var dayPlanDto = _mapper.Map<DayPlanDto>(dayPlan);
        return Result<DayPlanDto>.Success(dayPlanDto);
    }

    }
}
*/
