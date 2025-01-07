using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Application.Recipes;

namespace API.Controllers;

public class RecipesController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
    {
        return Ok(await Mediator.Send(new List.Query()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> GetRecipe(Guid id)
    {
        return await Mediator.Send(new Details.Query { Id = id });
    }

    [HttpPost]
    public async Task<ActionResult<Recipe>> CreateRecipe(Recipe recipe)
    {
        await Mediator.Send(new Create.Command {Recipe = recipe});
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Recipe>> UpdateRecipe(Guid id, Recipe recipe)
    {
        recipe.Id = id;
        await Mediator.Send(new Edit.Command {Recipe = recipe});
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Recipe>> DeleteRecipe(Guid id)
    {
        await Mediator.Send(new Delete.Command { Id = id });
        return Ok();
    }
}