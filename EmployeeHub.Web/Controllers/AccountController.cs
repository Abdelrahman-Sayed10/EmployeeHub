using EmployeeHub.Dtos.AuthDto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHub.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _client;

        public AccountController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("EmployeeHubApi");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (!ModelState.IsValid)
                return View(login);

            // Calling the Auth endpoint
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new { login.UserName, login.Password });
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(login);
            }

            // Reading the token
            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResult == null)
            {
                ModelState.AddModelError("", "Unknown error from server.");
                return View(login);
            }

            // Storing token in session
            HttpContext.Session.SetString("jwtToken", authResult.Token);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            if (!ModelState.IsValid)
                return View(register);

            var response = await _client.PostAsJsonAsync("/api/Auth/register", register);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Registration failed. Please try again.");
                return View(register);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return RedirectToAction("Login");

            // The "id_token" from OnCreatingTicket event
            var googleToken = result.Principal?.FindFirst("id_token")?.Value;
            if (string.IsNullOrEmpty(googleToken))
                return RedirectToAction("Login");

            var response = await _client.PostAsJsonAsync("/api/Auth/googleLogin", googleToken);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Google login failed.");
                return RedirectToAction("Login");
            }

            var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (authResult != null)
            {
                HttpContext.Session.SetString("jwtToken", authResult.Token);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("jwtToken");
            return RedirectToAction("Login", "Account");
        }
    }
}
