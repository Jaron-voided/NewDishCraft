using Application.Core;
using Domain.Enums;

namespace Application.Recipes;

public class RecipeParams : PagingParams
{
    public bool All { get; set; }
    public bool PriceHigh { get; set; }
    public bool PriceLow { get; set; }
    public Categories.RecipeCategory? RecipeCategory { get; set; }
    public string? UserId { get; set; }
}