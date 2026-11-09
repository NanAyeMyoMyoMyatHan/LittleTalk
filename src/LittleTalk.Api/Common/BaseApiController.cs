using Microsoft.AspNetCore.Mvc;
using LittleTalk.Model.Common;

namespace LittleTalk.Api.Common;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ToActionResult(Result result)
    {
        return result.ResultType switch
        {
            ResultType.Success => Ok(result),
            ResultType.NotFound => NotFound(result),
            ResultType.Duplicate => Conflict(result),
            ResultType.ValidationError => UnprocessableEntity(result),
            ResultType.Unauthorized => Unauthorized(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.ResultType switch
        {
            ResultType.Success => Ok(result),
            ResultType.NotFound => NotFound(result),
            ResultType.Duplicate => Conflict(result),
            ResultType.ValidationError => UnprocessableEntity(result),
            ResultType.Unauthorized => Unauthorized(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }

    protected IActionResult ToCreatedAtActionResult<T>(
        Result<T> result, 
        string actionName, 
        object routeValues)
    {
        if (result.IsSuccess)
        {
            return CreatedAtAction(actionName, routeValues, result);
        }

        return ToActionResult(result);
    }
}
