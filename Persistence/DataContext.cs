using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<NutritionalInfo> NutritionalInfos { get; set; }
}