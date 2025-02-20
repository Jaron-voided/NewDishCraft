using Application.Core;
using Domain.Enums;

namespace Application.Ingredients;

public class IngredientParams : PagingParams
{
    public bool All { get; set; }
    public bool PriceHigh { get; set; }
    public bool PriceLow { get; set; }
    public Categories.IngredientCategory? Category { get; set; }
    public string? UserId { get; set; }
}