using EmployeeClient.Services;
using EmployeeClient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeClient.Controllers
{
    [Authorize(Policy = "Employee")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> ClientUser(string userId)
        {
            try
            {
                var model = await _clientService.GetUser(userId);
                List<BillViewModel> bills;
                if (model.Role == "Client")
                {
                    bills = await _clientService.GetBills(userId);
                }
                else
                {
                    bills = new List<BillViewModel> { };
                }

                ProfileViewModel profileViewModel = new ProfileViewModel()
                {
                    ClientInfo = model,
                    BillsClient = bills,
                };
                return View(profileViewModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Непредвиденная ошибка в получении информации пользователя";
                return RedirectToAction("Index", "Employee");
            }
        }


        [HttpGet]
        public async Task<IActionResult> BlockUser(string userId)
        {
            try
            {
                await _clientService.BlockUser(userId);
                TempData["SuccesfullyMessage"] = "Пользователь успешно заблокирован";
                return RedirectToAction("ClientUser", "Client", new { userId = userId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Непредвиденная ошибка в блокировке";
                return RedirectToAction("ClientUser", "Employee", userId);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Clients()
        {
            try
            {
                var token = Request.Cookies["Bank.Identity.Cookie"];
                var model = await _clientService.GetUsers(token);
                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Непредвиденная ошибка в получении пользователей";
                return RedirectToAction("Index", "Employee");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Bill(long billId)
        {
            try
            {
                var bill = await _clientService.GetBill(billId);

                BillOperationsViewModel billOperations = new BillOperationsViewModel()
                {
                    Bill = bill,
                    Operations = new List<OperationViewModel>()
                };
                return View(billOperations);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Непредвиденная ошибка в получении счёта";
                return RedirectToAction("Index", "Employee");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreditInfo(long billId)
        {
            try
            {
                var model = await _clientService.GetCreditInformation(billId);

                return View(model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Непредвиденная ошибка в получении тарифа";
                return RedirectToAction("Index", "Employee");
            }
        }
    }
}