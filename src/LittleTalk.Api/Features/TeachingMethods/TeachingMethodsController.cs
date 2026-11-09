using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LittleTalk.Api.Common;
using LittleTalk.Application.Features.TeachingMethods.Services;
using LittleTalk.Model.Common;
using LittleTalk.Model.Features.TeachingMethods;

namespace LittleTalk.Api.Features.TeachingMethods;

[ApiController]
[Route("api/[controller]")]
[Route("api/teaching-methods")]
[Authorize(Roles = "Admin,Teacher")]
public class TeachingMethodsController : BaseApiController
{
    private readonly ITeachingMethodService _methodService;

    public TeachingMethodsController(ITeachingMethodService methodService)
    {
        _methodService = methodService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<List<TeachingMethodResponseModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken)
    {
        var result = await _methodService.GetAllMethodsAsync(isActive, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<TeachingMethodResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _methodService.GetMethodByIdAsync(id, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<TeachingMethodResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateTeachingMethodRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _methodService.CreateMethodAsync(request, cancellationToken);
        return ToCreatedAtActionResult(result, nameof(GetById), new { id = result.Data?.TeachingMethodId });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result<TeachingMethodResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTeachingMethodRequestModel request, CancellationToken cancellationToken)
    {
        var result = await _methodService.UpdateMethodAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _methodService.DeleteMethodAsync(id, cancellationToken);
        return ToActionResult(result);
    }
}
