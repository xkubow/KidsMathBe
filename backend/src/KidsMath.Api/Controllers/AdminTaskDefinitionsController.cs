using KidsMath.Api.Extensions;
using KidsMath.Application.Services;
using KidsMath.Contracts.MathTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsMath.Api.Controllers;

[ApiController]
[Authorize(Policy = "Admin")]
[Route("api/admin/task-definitions")]
public class AdminTaskDefinitionsController(MathTaskDefinitionAdminService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MathTaskDefinitionAdminDto>>> List(CancellationToken ct) =>
        Ok(await service.ListAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MathTaskDefinitionAdminDto>> Get(Guid id, CancellationToken ct)
    {
        var item = await service.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<MathTaskDefinitionAdminDto>> Create(CreateMathTaskDefinitionRequest request, CancellationToken ct)
    {
        var item = await service.CreateAsync(request, ct);
        return item is null ? BadRequest("Invalid task type.") : CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MathTaskDefinitionAdminDto>> Update(Guid id, UpdateMathTaskDefinitionRequest request, CancellationToken ct)
    {
        var item = await service.UpdateAsync(id, request, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
