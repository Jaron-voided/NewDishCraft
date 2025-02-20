using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Ingredients;

public class Create
{
    public class Command : IRequest<Result<IngredientDto>>
    {
        public IngredientDto IngredientDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.IngredientDto).SetValidator(new IngredientValidator());
        }
    }

    public class Handler : IRequestHandler<Command, Result<IngredientDto>>
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

        public async Task<Result<IngredientDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 Fetching the logged-in user...");
            Console.WriteLine("🔍 Fetching the logged-in user...");

            var username = _userAccessor.GetUsername();
            _logger.LogInformation($"✅ Retrieved username: {username}");
            Console.WriteLine($"✅ Retrieved username: {username}");

            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("❌ Unable to retrieve logged-in user");
                return Result<IngredientDto>.Failure("Unable to retrieve the logged-in user.");
            }

            var user = await _context.Users
                .Include(u => u.Ingredients)
                .FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning($"❌ User not found for username: {username}");
                Console.WriteLine($"❌ User not found for username: {username}");
                return Result<IngredientDto>.Failure("User not found");
            }

            _logger.LogInformation($"👤 User {user.Id} found with {user.Ingredients.Count} existing ingredients.");
            Console.WriteLine($"👤 User {user.Id} found with {user.Ingredients.Count} existing ingredients.");

            // ✅ Convert DTO to Entity
            var ingredient = _mapper.Map<Ingredient>(request.IngredientDto);
            ingredient.Id = Guid.NewGuid();
            ingredient.AppUser = user;
            ingredient.AppUserId = user.Id;
            
            if (ingredient.Nutrition != null)
            {
                ingredient.Nutrition.IngredientId = ingredient.Id;
                _context.NutritionalInfos.Add(ingredient.Nutrition);
            }

            user.Ingredients.Add(ingredient);

            _logger.LogInformation($"🛠️ Adding ingredient '{ingredient.Name}' for user {user.Id} with ID {ingredient.Id}...");
            Console.WriteLine($"🛠️ Adding ingredient '{ingredient.Name}' for user {user.Id} with ID {ingredient.Id}...");

            _context.Ingredients.Add(ingredient);

            if (ingredient.Nutrition != null)
                _context.NutritionalInfos.Add(ingredient.Nutrition);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                _logger.LogError("❌ Failed to save ingredient to the database.");
                Console.WriteLine("❌ Failed to save ingredient to the database.");
                return Result<IngredientDto>.Failure("Failed to create ingredient.");
            }

            _logger.LogInformation($"✅ Ingredient '{ingredient.Name}' successfully created with ID {ingredient.Id}.");
            Console.WriteLine($"✅ Ingredient '{ingredient.Name}' successfully created with ID {ingredient.Id}.");

            var ingredientDto = _mapper.Map<IngredientDto>(ingredient);
            return Result<IngredientDto>.Success(ingredientDto);
        }
    }
}
