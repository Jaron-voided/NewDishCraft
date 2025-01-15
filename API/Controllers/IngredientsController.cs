using Application.Ingredients;
using Domain;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;


namespace API.Controllers;

public class IngredientsController : BaseApiController
{

    [HttpGet]
    public async Task<IActionResult> GetIngredients()
    {
        return HandleResult(await Mediator.Send(new List.Query()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIngredient(Guid id)
    {
        return HandleResult(await Mediator.Send(new Details.Query {Id = id}));
    }

    [HttpPost]
    public async Task<ActionResult<Ingredient>> CreateIngredient(Ingredient ingredient)
    {
        return HandleResult(await Mediator.Send(new Create.Command { Ingredient = ingredient }));
        /*await Mediator.Send(new Create.Command {Ingredient = ingredient});
        return Ok();*/
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Ingredient>> EditIngredient(Guid id, Ingredient ingredient)
    {
        ingredient.Id = id;
        return HandleResult(await Mediator.Send(new Edit.Command {Ingredient = ingredient}));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Ingredient>> DeleteIngredient(Guid id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command {Id = id}));
        /*return Ok();*/
    }
}