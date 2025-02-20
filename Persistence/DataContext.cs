using System.Collections.Immutable;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DataContext : IdentityDbContext<AppUser>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<NutritionalInfo> NutritionalInfos { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    public DbSet<DayPlanRecipe> DayPlanRecipes { get; set; }
    public DbSet<DayPlan> DayPlans { get; set; }
    public DbSet<WeekPlan> WeekPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Recipe to User
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.AppUser)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ingredient to User
        modelBuilder.Entity<Ingredient>()
            .HasOne(i => i.AppUser)
            .WithMany(u => u.Ingredients)
            .HasForeignKey(i => i.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Recipe -> Measurements: Cascade delete
        modelBuilder.Entity<Measurement>()
            .HasOne(m => m.Recipe)
            .WithMany(r => r.Measurements)
            .HasForeignKey(m => m.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ingredient -> Measurements: Changed from Restrict to Cascade delete
        modelBuilder.Entity<Measurement>()
            .HasOne(m => m.Ingredient)
            .WithMany()
            .HasForeignKey(m => m.IngredientId)
            .OnDelete(DeleteBehavior.Cascade); // Changed from Restrict to Cascade

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

        // DayPlanRecipe -> DayPlan (Many-to-One)
        modelBuilder.Entity<DayPlanRecipe>()
            .HasOne(dpr => dpr.DayPlan)
            .WithMany(dp => dp.DayPlanRecipes)
            .HasForeignKey(dpr => dpr.DayPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // DayPlanRecipe -> Recipe (Many-to-One): Consider changing to Cascade
        modelBuilder.Entity<DayPlanRecipe>()
            .HasOne(dpr => dpr.Recipe)
            .WithMany()
            .HasForeignKey(dpr => dpr.RecipeId)
            .OnDelete(DeleteBehavior.Restrict); // Consider changing to Cascade if appropriate

        // WeekPlan -> DayPlans (One-to-Many) using WeekPlanId in DayPlan
        modelBuilder.Entity<WeekPlan>()
            .HasMany(wp => wp.DayPlans)
            .WithOne(dp => dp.WeekPlan)
            .HasForeignKey(dp => dp.WeekPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // DayPlan -> AppUser
        modelBuilder.Entity<DayPlan>()
            .HasOne(dp => dp.AppUser)
            .WithMany(u => u.DayPlans)
            .HasForeignKey(dp => dp.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // WeekPlan -> AppUser
        modelBuilder.Entity<WeekPlan>()
            .HasOne(wp => wp.AppUser)
            .WithMany(u => u.WeekPlans)
            .HasForeignKey(wp => wp.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/*
namespace Persistence;

public class DataContext : IdentityDbContext<AppUser>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<NutritionalInfo> NutritionalInfos { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    public DbSet<DayPlanRecipe> DayPlanRecipes { get; set; }
    public DbSet<DayPlan> DayPlans { get; set; }
    public DbSet<WeekPlan> WeekPlans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Recipe to User
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.AppUser)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ingredient to User
        modelBuilder.Entity<Ingredient>()
            .HasOne(i => i.AppUser)
            .WithMany(u => u.Ingredients)
            .HasForeignKey(i => i.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .OnDelete(DeleteBehavior.Cascade);

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

        // DayPlanRecipe -> DayPlan (Many-to-One)
        modelBuilder.Entity<DayPlanRecipe>()
            .HasOne(dpr => dpr.DayPlan)
            .WithMany(dp => dp.DayPlanRecipes)
            .HasForeignKey(dpr => dpr.DayPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // DayPlanRecipe -> Recipe (Many-to-One)
        modelBuilder.Entity<DayPlanRecipe>()
            .HasOne(dpr => dpr.Recipe)
            .WithMany()
            .HasForeignKey(dpr => dpr.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);

        // WeekPlan -> DayPlans (One-to-Many) using WeekPlanId in DayPlan
        modelBuilder.Entity<WeekPlan>()
            .HasMany(wp => wp.DayPlans)
            .WithOne(dp => dp.WeekPlan)
            .HasForeignKey(dp => dp.WeekPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // DayPlan -> AppUser
        modelBuilder.Entity<DayPlan>()
            .HasOne(dp => dp.AppUser)
            .WithMany(u => u.DayPlans)
            .HasForeignKey(dp => dp.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // WeekPlan -> AppUser
        modelBuilder.Entity<WeekPlan>()
            .HasOne(wp => wp.AppUser)
            .WithMany(u => u.WeekPlans)
            .HasForeignKey(wp => wp.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
*/




















/*using System.Collections.Immutable;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DataContext : IdentityDbContext<AppUser>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<NutritionalInfo> NutritionalInfos { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    public DbSet<DayPlanRecipe> DayPlanRecipes { get; set; }
    public DbSet<DayPlan> DayPlans { get; set; }
    public DbSet<WeekPlan> WeekPlan { get; set; }
    
    
    //public DbSet<AppUser> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Recipe to User
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.AppUser)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.AppUserId)
            .OnDelete(DeleteBehavior.Cascade); // Optional: Set delete behavior

        // Ingredient to User
        modelBuilder.Entity<Ingredient>()
            .HasOne(i => i.AppUser)
            .WithMany(u => u.Ingredients)
            .HasForeignKey(i => i.AppUserId)
            .OnDelete(DeleteBehavior.Cascade); // Optional: Set delete behavior

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
        
        // DayPlanRecipe -> DayPlan (Many-to-One)
        modelBuilder.Entity<DayPlanRecipe>()
            .HasOne(dpr => dpr.DayPlan)
            .WithMany(dp => dp.DayPlanRecipes)
            .HasForeignKey(dpr => dpr.DayPlanId)
            .OnDelete(DeleteBehavior.Cascade); // ✅ If a DayPlan is deleted, its recipes are deleted

        // DayPlanRecipe -> Recipe (Many-to-One)
        modelBuilder.Entity<DayPlanRecipe>()
            .HasOne(dpr => dpr.Recipe)
            .WithMany()
            .HasForeignKey(dpr => dpr.RecipeId)
            .OnDelete(DeleteBehavior.Restrict); // ✅ A Recipe should NOT be deleted if it's in a meal plan
        
        // WeekPlan -> DayPlans (One-to-Many)
        modelBuilder.Entity<WeekPlan>()
            .HasMany(wp => wp.DayPlans)
            .WithOne()
            .HasForeignKey(dp => dp.Id)
            .OnDelete(DeleteBehavior.Cascade); // ✅ If a WeekPlan is deleted, all DayPlans inside it are deleted
        
        modelBuilder.Entity<DayPlan>()
            .HasOne(dp => dp.AppUser)
            .WithMany(u => u.DayPlans)
            .HasForeignKey(dp => dp.AppUserId)
            .OnDelete(DeleteBehavior.Cascade); // ✅ Cascade delete when user is deleted

        modelBuilder.Entity<WeekPlan>()
            .HasOne(wp => wp.AppUser)
            .WithMany(u => u.WeekPlans)
            .HasForeignKey(wp => wp.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}*/