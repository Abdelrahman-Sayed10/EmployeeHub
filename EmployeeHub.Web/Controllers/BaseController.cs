using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace EmployeeHub.Web.Controllers
{
    public class BaseController : Controller
    {
        protected HttpClient client = new HttpClient();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);

                    var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "userId");
                    var isAdminClaim = jwt.Claims.FirstOrDefault(c => c.Type == "isAdmin");
                    var subClaim = jwt.Claims.FirstOrDefault(c => c.Type == "sub");

                    ViewBag.CurrentUserId = userIdClaim?.Value;
                    ViewBag.CurrentUserIsAdmin = string.Equals(isAdminClaim?
                                                                    .Value, "True", StringComparison.OrdinalIgnoreCase);
                    ViewBag.CurrentUserName = subClaim?.Value;

                    client.BaseAddress = new Uri("http://localhost:5049/");
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                catch (Exception)
                {
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
