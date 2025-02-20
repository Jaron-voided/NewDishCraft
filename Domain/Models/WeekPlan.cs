using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Models;

public class WeekPlan
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public ICollection<DayPlan> DayPlans { get; set; } = new List<DayPlan>();
    
    [JsonIgnore]
    public AppUser? AppUser { get; set; } // Navigation property
    [JsonIgnore]
    public string? AppUserId { get; set; } // Foreign key
}