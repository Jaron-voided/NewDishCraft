using Application.Core;
using Application.Ingredients;
using Domain;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;


namespace API.Controllers;

[Authorize]
public class IngredientsController : BaseApiController
{
    private readonly ILogger<IngredientsController> _logger;

    public IngredientsController(ILogger<IngredientsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetIngredients([FromQuery]IngredientParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query{Params = param}));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIngredient(Guid id)
    {
        return HandleResult(await Mediator.Send(new Details.Query {Id = id}));
    }
    
    [HttpGet("{id}/nutrition")]
    public async Task<IActionResult> GetIngredientNutrition(Guid id)
    {
        return HandleResult(await Mediator.Send(new NutritionDetails.Query { Id = id }));
    }
 
    [HttpPost]
    public async Task<ActionResult<IngredientDto>> CreateIngredient(IngredientDto ingredientDto)
    {
        // ✅ Log every field to check for null values
        Console.WriteLine($"Received IngredientDto:");
        Console.WriteLine($"  - Name: {ingredientDto?.Name ?? "NULL"}");
        Console.WriteLine($"  - Category: {ingredientDto?.Category ?? "NULL"}");
        Console.WriteLine($"  - MeasuredIn: {ingredientDto?.MeasuredIn ?? "NULL"}");
        Console.WriteLine($"  - PricePerPackage: {ingredientDto?.PricePerPackage ?? 0}");
        Console.WriteLine($"  - MeasurementsPerPackage: {ingredientDto?.MeasurementsPerPackage ?? 0}");
        Console.WriteLine($"  - Calories: {ingredientDto?.Nutrition.Calories ?? 0}");
        Console.WriteLine($"  - Carbs: {ingredientDto?.Nutrition.Carbs ?? 0}");
        Console.WriteLine($"  - Fat: {ingredientDto?.Nutrition.Fat ?? 0}");
        Console.WriteLine($"  - Protein: {ingredientDto?.Nutrition.Protein ?? 0}");
        Console.WriteLine($"  - AppUserId: {ingredientDto?.AppUserId ?? "NULL"}");

        if (ingredientDto == null)
        {
            Console.WriteLine("❌ ERROR: ingredientDto is NULL!");
            _logger.LogWarning("Received a NULL ingredientDto");
            return BadRequest("Ingredient data is missing.");
        }

        _logger.LogInformation("📨 Received request to create an ingredient. Name: {IngredientName}, Category: {Category}",
            ingredientDto.Name, ingredientDto.Category);
        Console.WriteLine($"Adding ingredientDto '{ingredientDto.Name}' for user {ingredientDto.AppUserId}...");

        // ✅ Send the request to the Mediator (Handler will process it)
        var result = await Mediator.Send(new Create.Command { IngredientDto = ingredientDto });

        // ✅ Handle failure scenario
        if (!result.IsSuccess)
        {
            _logger.LogWarning("❌ Failed to create ingredient. Error: {Error}", result.Error);
            Console.WriteLine($"❌ ERROR: Failed to create ingredient. Reason: {result.Error}");
            return HandleResult(result);
        }

        // ✅ Successful creation
        _logger.LogInformation("✅ Successfully created ingredient: {IngredientName}, ID: {IngredientId}",
            result.Value.Name, result.Value.Id);
        Console.WriteLine($"✅ Ingredient '{result.Value.Name}' created successfully with ID {result.Value.Id}.");

        Console.WriteLine($"Returning ingredient with ID: {result.Value.Id}");

        return CreatedAtAction(nameof(CreateIngredient), new { id = result.Value.Id }, result.Value);
    }
    

    [HttpPut("{id}")]
    public async Task<ActionResult<Ingredient>> EditIngredient(Guid id, IngredientDto ingredientDto)
    {
        ingredientDto.Id = id;
        return HandleResult(await Mediator.Send(new Edit.Command {IngredientDto = ingredientDto}));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Ingredient>> DeleteIngredient(Guid id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command {Id = id}));
    }
}