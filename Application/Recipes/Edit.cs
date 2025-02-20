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

public class Edit
{
    public class Command : IRequest<Result<Unit>>
    {
        //public Guid Id { get; set; }
        public RecipeDto RecipeDto { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.RecipeDto).SetValidator(new RecipeValidator());
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
            _logger.LogInformation($"🔍 Fetching Recipe with ID: {request.RecipeDto.Id}");
            Console.WriteLine($"🔍 Fetching Recipe with ID: {request.RecipeDto.Id}");

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == request.RecipeDto.Id, cancellationToken);

            if (recipe == null)
            {
                _logger.LogWarning($"❌ Recipe with ID {request.RecipeDto.Id} not found.");
                Console.WriteLine($"❌ Recipe with ID {request.RecipeDto.Id} not found.");
                return Result<Unit>.Failure("Recipe not found");
            }

            var username = _userAccessor.GetUsername();
            _logger.LogInformation($"✅ Retrieved username: {username}");
            Console.WriteLine($"✅ Retrieved username: {username}");

            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("❌ Unable to retrieve the logged-in user.");
                return Result<Unit>.Failure("Unauthorized");
            }

            // **Ensure the user owns the recipe**
            if (recipe.AppUserId != request.RecipeDto.AppUserId)
            {
                _logger.LogWarning($"❌ Unauthorized user {username} attempted to edit recipe {recipe.Id}");
                return Result<Unit>.Failure("Unauthorized");
            }

            // **Map changes from DTO**
            _mapper.Map(request.RecipeDto, recipe);

            _logger.LogInformation($"🛠️ Updating recipe '{recipe.Name}'...");
            Console.WriteLine($"🛠️ Updating recipe '{recipe.Name}'...");

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                _logger.LogError($"❌ Failed to update recipe '{recipe.Name}'.");
                Console.WriteLine($"❌ Failed to update recipe '{recipe.Name}'.");
                return Result<Unit>.Failure("Failed to update recipe.");
            }

            _logger.LogInformation($"✅ Recipe '{recipe.Name}' successfully updated.");
            Console.WriteLine($"✅ Recipe '{recipe.Name}' successfully updated.");

            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to update recipe.");
            /*
            return Result<Unit>.Success(_mapper.Map(request.RecipeDto, recipe));
        */
        }
    }
}
