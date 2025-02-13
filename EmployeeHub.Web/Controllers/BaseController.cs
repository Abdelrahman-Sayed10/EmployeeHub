using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeHub.Web.Controllers
{
    public class BaseController : Controller
    {
        protected HttpClient client = new HttpClient();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            client.BaseAddress = new Uri("http://localhost:5049/");
            base.OnActionExecuting(context);
        }
    }
}
