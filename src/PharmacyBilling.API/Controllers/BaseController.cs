using Microsoft.AspNetCore.Mvc;
using PharmacyBilling.Application.Common.Models;

namespace PharmacyBilling.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(new { success = true, data = result.Data });
        return result.StatusCode switch
        {
            401 => Unauthorized(new { success = false, error = result.Error }),
            403 => StatusCode(403, new { success = false, error = result.Error }),
            404 => NotFound(new { success = false, error = result.Error }),
            409 => Conflict(new { success = false, error = result.Error }),
            _ => BadRequest(new { success = false, error = result.Error })
        };
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return Ok(new { success = true });
        return result.StatusCode switch
        {
            401 => Unauthorized(new { success = false, error = result.Error }),
            403 => StatusCode(403, new { success = false, error = result.Error }),
            404 => NotFound(new { success = false, error = result.Error }),
            409 => Conflict(new { success = false, error = result.Error }),
            _ => BadRequest(new { success = false, error = result.Error })
        };
    }
}
