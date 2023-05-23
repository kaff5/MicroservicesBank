using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeClient.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}