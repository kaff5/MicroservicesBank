using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsersService.Data;
using UsersService.Models;
using UsersService.ViewModels;

namespace UsersService.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public AuthController(ApplicationDbContext dbcontext, UserManager<User> userManager,
            SignInManager<User> signInManager, IIdentityServerInteractionService identityServerInteractionService)
        {
            _dbContext = dbcontext;
            _userManager = userManager;
            _signInManager = signInManager;
            _identityServerInteractionService = identityServerInteractionService;
        }

        [HttpGet]
        public IActionResult Login([FromQuery] string ReturnUrl)
        {
            var viewModel = new LoginViewModel
            {
                RedirectUrl = ReturnUrl,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(model.RedirectUrl ?? "https://www.youtube.com/watch?v=dQw4w9WgXcQ");
                    }
                }

                ModelState.AddModelError("", "Incorrect username or password");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutRequest = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
    }
}