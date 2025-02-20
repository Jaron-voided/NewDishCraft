using Application.Measurements;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;
namespace API.Controllers;

public class MeasurementsController : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MeasurementDto>>> GetMeasurements()
    {
        return Ok(await Mediator.Send(new List.Query()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MeasurementDto>> GetMeasurement(Guid id)
    {
        return Ok(await Mediator.Send(new Details.Query { Id = id }));
    }

    [HttpGet("by-recipe/{recipeId}")]
    public async Task<ActionResult<IEnumerable<Measurement>>> GetMeasurementsByRecipe(Guid recipeId)
    {
        return Ok(await Mediator.Send(new ListByRecipe.Query { RecipeId = recipeId }));
    }

    [HttpPost]
    public async Task<ActionResult<Measurement>> CreateMeasurement(Measurement measurement)
    {
        await Mediator.Send(new Create.Command 
        { 
            RecipeId = measurement.RecipeId, 
            IngredientId = measurement.IngredientId, 
            Amount = measurement.Amount
        });
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Measurement>> UpdateMeasurement(Guid id, Measurement measurement)
    {
        measurement.Id = id;
        await Mediator.Send(new Edit.Command
        {
            RecipeId = measurement.RecipeId,
            IngredientId = measurement.IngredientId,
            Amount = measurement.Amount
        });
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMeasurement(Guid id)
    {
        await Mediator.Send(new Delete.Command { Id = id });
        return Ok();
    }
    
    
}