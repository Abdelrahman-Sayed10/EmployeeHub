using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Dtos.AuthDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _authService.Register(registerDto);
            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.Login(loginDto);
            return Ok(response);
        }

        [HttpPost("googleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] string googleToken)
        {
            var response = await _authService.LoginWithGoogle(googleToken);
            return Ok(response);
        }
    }
}
