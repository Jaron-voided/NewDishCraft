using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Recipes;

public class Create
{
    public class Command : IRequest<Result<RecipeDto>>
    {
        public RecipeDto RecipeDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.RecipeDto.Name).NotEmpty();
            RuleFor(x => x.RecipeDto.RecipeCategory).NotEmpty();
            RuleFor(x => x.RecipeDto.ServingsPerRecipe).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<Command, Result<RecipeDto>>
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

        public async Task<Result<RecipeDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            try 
            {
                _logger.LogInformation("🔍 Fetching the logged-in user...");
                Console.WriteLine("🔍 Fetching the logged-in user...");

                var username = _userAccessor.GetUsername();
                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("❌ Unable to retrieve logged-in user");
                    Console.WriteLine("❌ Unable to retrieve logged-in user");
                    return Result<RecipeDto>.Failure("Unable to retrieve the logged-in user.");
                }

                var user = await _context.Users
                    .Include(u => u.Recipes)
                    .FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning($"❌ User not found for username: {username}");
                    Console.WriteLine($"❌ User not found for username: {username}");
                    return Result<RecipeDto>.Failure("User not found");
                }

                _logger.LogInformation($"👤 User {user.Id} found with {user.Recipes.Count} existing recipes.");
                Console.WriteLine($"👤 User {user.Id} found with {user.Recipes.Count} existing recipes.");

                // Create recipe with new ID
                var recipe = _mapper.Map<Recipe>(request.RecipeDto);
                recipe.Id = Guid.NewGuid();
                recipe.AppUser = user;
                recipe.AppUserId = user.Id;

                _logger.LogInformation($"🛠️ Mapping measurements for recipe '{recipe.Name}'...");
                
                // Ensure measurements are properly initialized
                if (recipe.Measurements != null)
                {
                    foreach (var measurement in recipe.Measurements)
                    {
                        measurement.Id = Guid.NewGuid();
                        measurement.RecipeId = recipe.Id;
                        _logger.LogInformation($"📏 Created measurement: {measurement.Amount} of ingredient {measurement.IngredientId}");
                    }
                }

                user.Recipes.Add(recipe);
                _context.Recipes.Add(recipe);

                _logger.LogInformation($"🛠️ Adding recipe '{recipe.Name}' for user {user.Id} with ID {recipe.Id}...");
                Console.WriteLine($"🛠️ Adding recipe '{recipe.Name}' for user {user.Id} with ID {recipe.Id}...");

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    _logger.LogError("❌ Failed to save recipe to the database.");
                    Console.WriteLine("❌ Failed to save recipe to the database.");
                    return Result<RecipeDto>.Failure("Failed to create recipe.");
                }

                // Load the complete recipe with all related data for the return DTO
                var createdRecipe = await _context.Recipes
                    .Include(r => r.Measurements)
                        .ThenInclude(m => m.Ingredient)
                            .ThenInclude(i => i.Nutrition)
                    .FirstOrDefaultAsync(r => r.Id == recipe.Id, cancellationToken);

                if (createdRecipe == null)
                {
                    _logger.LogError("❌ Failed to retrieve created recipe after saving.");
                    return Result<RecipeDto>.Failure("Failed to retrieve created recipe.");
                }

                _logger.LogInformation($"✅ Recipe '{recipe.Name}' successfully created with ID {recipe.Id}.");
                Console.WriteLine($"✅ Recipe '{recipe.Name}' successfully created with ID {recipe.Id}.");

                var recipeDto = _mapper.Map<RecipeDto>(createdRecipe);
                return Result<RecipeDto>.Success(recipeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating recipe: {Message}", ex.Message);
                return Result<RecipeDto>.Failure($"Error creating recipe: {ex.Message}");
            }
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
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Recipes;

public class Create
{
    public class Command : IRequest<Result<RecipeDto>>
    {
        public RecipeDto RecipeDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.RecipeDto.Name).NotEmpty();
            RuleFor(x => x.RecipeDto.RecipeCategory).NotEmpty();
            RuleFor(x => x.RecipeDto.ServingsPerRecipe).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<Command, Result<RecipeDto>>
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

        public async Task<Result<RecipeDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 Fetching the logged-in user...");
            Console.WriteLine("🔍 Fetching the logged-in user...");

            var username = _userAccessor.GetUsername();
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("❌ Unable to retrieve logged-in user");
                return Result<RecipeDto>.Failure("Unable to retrieve the logged-in user.");
            }

            var user = await _context.Users
                .Include(u => u.Recipes)
                .FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"❌ User not found for username: {username}");
                Console.WriteLine($"❌ User not found for username: {username}");
                return Result<RecipeDto>.Failure("User not found");
            }

            _logger.LogInformation($"👤 User {user.Id} found with {user.Recipes.Count} existing recipes.");
            Console.WriteLine($"👤 User {user.Id} found with {user.Recipes.Count} existing recipes.");

            // ✅ Convert DTO to Entity
            var recipe = _mapper.Map<Recipe>(request.RecipeDto);
            recipe.Id = Guid.NewGuid();
            recipe.AppUser = user;
            recipe.AppUserId = user.Id;

            user.Recipes.Add(recipe);

            _logger.LogInformation($"🛠️ Adding recipe '{recipe.Name}' for user {user.Id} with ID {recipe.Id}...");
            Console.WriteLine($"🛠️ Adding recipe '{recipe.Name}' for user {user.Id} with ID {recipe.Id}...");

            _context.Recipes.Add(recipe);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                _logger.LogError("❌ Failed to save recipe to the database.");
                Console.WriteLine("❌ Failed to save recipe to the database.");
                return Result<RecipeDto>.Failure("Failed to create recipe.");
            }

            _logger.LogInformation($"✅ Recipe '{recipe.Name}' successfully created with ID {recipe.Id}.");
            Console.WriteLine($"✅ Recipe '{recipe.Name}' successfully created with ID {recipe.Id}.");

            var recipeDto = _mapper.Map<RecipeDto>(recipe);
            return Result<RecipeDto>.Success(recipeDto);
        }
    }
}
*/
