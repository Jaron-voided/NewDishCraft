using System.Reflection.Metadata;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Application.Recipes;
using MediatR;

namespace API.Controllers;

public class RecipesController : BaseApiController
{
    private readonly ILogger<RecipesController> _logger;
    public RecipesController(ILogger<RecipesController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRecipes([FromQuery] RecipeParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query{Params = param}));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(Guid id)
    {
        var result = await Mediator.Send(new Details.Query { Id = id });

        if (result == null) return NotFound(); // ✅ Ensure 404 is returned for missing recipes

        return HandleResult(result);
        //return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDto>> CreateRecipe(RecipeDto recipeDto)
    {
        // ✅ Log input for debugging
        Console.WriteLine("📨 Received request to create an recipe.");
        Console.WriteLine($"  - Name: {recipeDto?.Name ?? "NULL"}");
        Console.WriteLine($"  - Category: {recipeDto?.RecipeCategory ?? "NULL"}");
        Console.WriteLine($"  - Servings: {recipeDto?.ServingsPerRecipe ?? 0}");
        Console.WriteLine($"  - Instructions: {string.Join(", ", recipeDto?.Instructions ?? new List<string> { "NULL" })}");
        Console.WriteLine($"  - AppUserId: {recipeDto?.AppUserId ?? "NULL"}");

        if (recipeDto == null)
        {
            Console.WriteLine("❌ ERROR: recipeDto is NULL!");
            _logger.LogWarning("Received a NULL recipeDto");
            return BadRequest("Recipe data is missing.");
        }

        _logger.LogInformation("📨 Received request to create a recipe. Name: {RecipeName}, Category: {Category}",
            recipeDto.Name, recipeDto.RecipeCategory);
        Console.WriteLine($"Adding recipeDto '{recipeDto.Name}' for user {recipeDto.AppUserId}...");

        // ✅ Send to Mediator
        var result = await Mediator.Send(new Create.Command { RecipeDto = recipeDto });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("❌ Failed to create recipe. Error: {Error}", result.Error);
            Console.WriteLine($"❌ ERROR: Failed to create recipe. Reason: {result.Error}");
            return HandleResult(result);
        }

        // ✅ Successfully created
        _logger.LogInformation("✅ Successfully created recipe: {RecipeName}, ID: {RecipeId}",
            result.Value.Name, result.Value.Id);
        Console.WriteLine($"✅ Recipe '{result.Value.Name}' created successfully with ID {result.Value.Id}.");

        return CreatedAtAction(nameof(CreateRecipe), new { id = result.Value.Id }, result.Value);
    }
    

    [HttpPut("{id}")]
    public async Task<ActionResult<Recipe>> UpdateRecipe(Guid id, RecipeDto recipeDto)
    {
        recipeDto.Id = id;
        return HandleResult(await Mediator.Send(new Edit.Command { RecipeDto = recipeDto }));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRecipe(Guid id)
    {
        var result = await Mediator.Send(new Delete.Command { Id = id });

        if (!result.IsSuccess)
        {
            if (result.Error == "Not Found") 
            {
                Console.WriteLine($"❌ DELETE request for Recipe {id} returned 404 Not Found.");
                return NotFound(); // ✅ Return 404 if not found
            }
            return BadRequest(result.Error);
        }

        Console.WriteLine($"✅ DELETE request for Recipe {id} returned 200 OK.");
        return Ok();
    }

}