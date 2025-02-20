using Application.DayPlan.DTOs;
using FluentValidation;

namespace Application.DayPlan;

public class DayPlanValidator : AbstractValidator<DayPlanDto>
{
    public DayPlanValidator()
    {
        RuleFor(dp => dp.Name)
            .NotEmpty().WithMessage("DayPlan name is required.");

        RuleFor(dp => dp.AppUserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(dp => dp.DayPlanRecipes)
            .NotEmpty().WithMessage("A DayPlan must contain at least one recipe.");

        RuleForEach(dp => dp.DayPlanRecipes)
            .ChildRules(recipe =>
            {
                recipe.RuleFor(r => r.Servings)
                    .GreaterThan(0).WithMessage("Each recipe must have at least 1 serving.");
                
                recipe.RuleFor(r => r.RecipeId)
                    .NotEmpty().WithMessage("Recipe ID is required.");
            });
    }
}