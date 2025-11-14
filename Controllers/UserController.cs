using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoxiasApp.Utils;

namespace VoxiasApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var userId = User.GetUserId();
        var email = User.GetEmail();

        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Token não contém UserId válido." });

        return Ok(new
        {
            userId,
            email
        });
    }
}
