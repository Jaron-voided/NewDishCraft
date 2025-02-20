using Application.Core;
using Domain.Enums;

namespace Application.DayPlans;

public class DayPlanParams : PagingParams
{
    public bool All { get; set; }
    public bool PriceHigh { get; set; }
    public bool PriceLow { get; set; }
    public string? UserId { get; set; }
}