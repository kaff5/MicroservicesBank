using EmployeeClient.Services;
using EmployeeClient.ViewModels;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeClient.Controllers
{
    [Authorize(Policy = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            var ff = User.Claims;
            return View();
        }

        public IActionResult RegisterEmployee()
        {
            return View();
        }

        public IActionResult RegisterClient()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployee(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userModel = await _employeeService.RegisterEmployee(model);
                    TempData["SuccesfullyMessage"] = "Сотрудник был зарегистрирован";
                    return RedirectToAction("Index", "Employee");
                }
                catch (RpcException ex)
                {
                    TempData["ErrorMessage"] = "Сотрудник не был зарегистрирован";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Непредвиденная ошибка в регистрации сотрудника";
                }

                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterClient(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userModel = await _employeeService.RegisterClient(model);
                    TempData["SuccesfullyMessage"] = "Клиент был зарегистрирован";
                    return RedirectToAction("ClientUser", "Client", new { userId = userModel.UserId });
                }
                catch (RpcException ex)
                {
                    TempData["ErrorMessage"] = "Клиент не был зарегистрирован";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Непредвиденная ошибка в регистрации клиента";
                }

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateTariff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTariff(CreateTariffViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.CreateTariff(model);
                    TempData["SuccesfullyMessage"] = "Тариф был создан";
                    return RedirectToAction("Index", "Employee");
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Непредвиденная ошибка в создании тарифа";
                    return RedirectToAction("Index", "Employee");
                }
            }

            return View(model);
        }
    }
}