using Application.DayPlan;
using Application.DayPlan.DTOs;
using Application.DayPlans;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers;

[Authorize]
public class DayPlanController : BaseApiController
{
    private readonly ILogger<DayPlanController> _logger;

    public DayPlanController(ILogger<DayPlanController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetDayPlans([FromQuery] DayPlanParams param)
    {
        return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DayPlanDto>> GetDayPlan(Guid id)
    {
        var result = await Mediator.Send(new Details.Query { Id = id });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("❌ Failed to retrieve DayPlan with ID: {Id}. Error: {Error}", id, result.Error);
            return HandleResult(result);
        }

        return Ok(result.Value);
    }

    //[AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<DayPlanDto>> AddDayPlan(DayPlanDto dayPlanDto)
    {
        if (dayPlanDto == null)
        {
            _logger.LogWarning("❌ Received a NULL dayPlanDto");
            return BadRequest("Day plan data is missing.");
        }

        _logger.LogInformation("📨 Received request to create a DayPlan: {Name}", dayPlanDto.Name);

        var result = await Mediator.Send(new Create.Command { DayPlanDto = dayPlanDto });

        if (!result.IsSuccess)
        {
            _logger.LogWarning("❌ Failed to create DayPlan. Error: {Error}", result.Error);
            return HandleResult(result);
        }

        _logger.LogInformation("✅ Successfully created DayPlan with ID: {Id}", result.Value.Id);
        
        return CreatedAtAction(nameof(GetDayPlan), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DayPlan>> UpdateDayPlan(Guid id, DayPlanDto dayPlanDto)
    {
        dayPlanDto.Id = id;
        return HandleResult(await Mediator.Send(new Edit.Command { DayPlanDto = dayPlanDto }));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDayPlan(Guid id)
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
    
    [HttpGet("{id}/calculations")]
    public async Task<IActionResult> GetDayPlanCalculations(Guid id)
    {
        _logger.LogInformation("📊 Received request to calculate totals for DayPlan: {Id}", id);

        var result = await Mediator.Send(new CalculateDayPlanTotals.Query { DayPlanId = id });

        return HandleResult(result);
    }
}