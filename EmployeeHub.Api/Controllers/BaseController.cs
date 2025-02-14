using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EmployeeHub.Api.Controllers;

[Authorize]
[ApiController]
public class BaseController : ControllerBase
{
    protected string? CurrentUserId => User.FindFirst("userId")?.Value;

    protected bool CurrentUserIsAdmin => User.FindFirst("isAdmin")?.Value == "True";

    protected string? CurrentUserName
        => User.FindFirst(ClaimTypes.Name)?.Value
           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
           ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
}
