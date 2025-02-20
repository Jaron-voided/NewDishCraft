using FluentValidation;
using Application.Recipes;

public class RecipeValidator : AbstractValidator<RecipeDto>
{
    public RecipeValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Recipe name is required.")
            .MaximumLength(200).WithMessage("Recipe name must be 200 characters or fewer.");

        RuleFor(r => r.RecipeCategory)
            .NotEmpty().WithMessage("Recipe category is required.");

        RuleFor(r => r.ServingsPerRecipe)
            .GreaterThan(0).WithMessage("Servings per recipe must be greater than zero.");

        RuleFor(r => r.Instructions)
            .NotNull().WithMessage("Instructions are required.")
            .Must(i => i.Count > 0).WithMessage("At least one instruction step is required.");
    }
}