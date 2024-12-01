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
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Recipe -> Measurements: Cascade delete
        modelBuilder.Entity<Measurement>()
            .HasOne(m => m.Recipe)
            .WithMany(r => r.Measurements)
            .HasForeignKey(m => m.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ingredient -> Measurements: Restrict delete
        modelBuilder.Entity<Measurement>()
            .HasOne(m => m.Ingredient)
            .WithMany()
            .HasForeignKey(m => m.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ingredient -> NutritionalInfo: Cascade delete
        modelBuilder.Entity<NutritionalInfo>()
            .HasOne(n => n.Ingredient)
            .WithOne(i => i.Nutrition)
            .HasForeignKey<NutritionalInfo>(n => n.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ingredient -> MeasurementUnit: Cascade delete
        modelBuilder.Entity<MeasurementUnit>()
            .HasOne(mu => mu.Ingredient)
            .WithOne(i => i.MeasurementUnit)
            .HasForeignKey<MeasurementUnit>(mu => mu.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}