using CoreClient;
using EmployeeClient.ViewModels;
using Grpc.Core;
using Grpc.Net.Client;
using LoanService;
using UsersService;

namespace EmployeeClient.Services
{
    public interface IClientService
    {
        Task<List<BillViewModel>> GetBills(string userId);
        Task<List<UserViewModel>> GetUsers(string token);
        Task BlockUser(string userId);
        Task<UserViewModel> GetUser(string userId);
        Task<BillViewModel> GetBill(long billId);
        Task<CreditViewModel> GetCreditInformation(long billId);
    }

    public class ClientService : IClientService
    {
        private readonly Users.UsersClient clientForUsers;
        private readonly BankCoreService.BankCoreServiceClient clientForCore;
        private readonly Loaner.LoanerClient clientForLoan;

        public ClientService()
        {
            var portCoreService = Environment.GetEnvironmentVariable("PortCoreService");
            var ipCoreService = Environment.GetEnvironmentVariable("IpCoreService");
            var portUsersService = Environment.GetEnvironmentVariable("PortUsersService");
            var ipUsersService = Environment.GetEnvironmentVariable("IpUsersService");

            var channelForUsers = GrpcChannel.ForAddress($"http://{ipUsersService}:{portUsersService}");
            clientForUsers = new Users.UsersClient(channelForUsers);

            var channelForCore = GrpcChannel.ForAddress($"http://{ipCoreService}:{portCoreService}");
            clientForCore = new BankCoreService.BankCoreServiceClient(channelForCore);
        }

        public async Task BlockUser(string userId)
        {
            var response = await clientForUsers.BlockUserByIdAsync(new UserRequestById()
            {
                UserId = userId
            });
        }

        public async Task<List<BillViewModel>> GetBills(string userId)
        {
            var response = await clientForCore.GetUserBillsAsync(
                new GetUserBillsRequest()
                {
                    UserId = userId
                }
            );

            List<BillViewModel> bills = new List<BillViewModel>();
            foreach (var bill in response.Bills)
            {
                ViewModels.BillType billType;
                if (bill.Type == CoreClient.BillType.Debit)
                {
                    billType = ViewModels.BillType.DEBIT;
                }
                else
                {
                    billType = ViewModels.BillType.CREDIT;
                }

                StatusBill billStatus;
                if (bill.Status == BillStatus.Opened)
                {
                    billStatus = StatusBill.OPENED;
                }
                else
                {
                    billStatus = StatusBill.CLOSED;
                }

                bills.Add(new BillViewModel
                {
                    Id = bill.Id,
                    Balance = bill.Balance,
                    Type = billType,
                    CreateAt = bill.CreatedAt,
                    Status = billStatus
                });
            }

            return bills;
        }

        public async Task<BillViewModel> GetBill(long billId)
        {
            var response = await clientForCore.GetBillAsync(
                new GetBillRequest()
                {
                    BillId = billId
                }
            );

            ViewModels.BillType billType;
            if (response.Type == CoreClient.BillType.Debit)
            {
                billType = ViewModels.BillType.DEBIT;
            }
            else
            {
                billType = ViewModels.BillType.CREDIT;
            }

            StatusBill billStatus;
            if (response.Status == BillStatus.Opened)
            {
                billStatus = StatusBill.OPENED;
            }
            else
            {
                billStatus = StatusBill.CLOSED;
            }

            return new BillViewModel
            {
                Id = response.Id,
                Balance = response.Balance,
                Type = billType,
                CreateAt = response.CreatedAt,
                Status = billStatus
            };
        }


        public async Task<List<UserViewModel>> GetUsers(string token)
        {
            var headers = new Metadata
            {
                { "Bank.Identity.Cookie", $"{token}" }
            };

            var response = await clientForUsers.GetAllUsersAsync(
                new GetAllUsersRequest(), headers
            );

            var listUsersViewModel = new List<UserViewModel>();

            foreach (var user in response.Users)
            {
                listUsersViewModel.Add(new UserViewModel
                {
                    Name = user.Name,
                    Patronymic = user.Patronymic,
                    Surname = user.Surname,
                    Username = user.UserName,
                    isBlocked = user.IsBlocked,
                    UserId = user.UserId,
                    Role = user.Role,
                });
            }

            return listUsersViewModel;
        }

        public async Task<UserViewModel> GetUser(string userId)
        {
            var response = await clientForUsers.GetUserByIdAsync(new UserRequestById
            {
                UserId = userId
            });

            var response2 = await clientForLoan.GetUserCreditRatingAsync(new RequestCreditRatingModel
            {
                UserId = userId
            });

            return new UserViewModel
            {
                Name = response.Name,
                isBlocked = response.IsBlocked,
                Patronymic = response.Patronymic,
                Surname = response.Surname,
                UserId = response.UserId,
                Username = response.UserName,
                Role = response.Role,
                CreditRating = response2.Rating.ToString()
            };
        }

        public async Task<CreditViewModel> GetCreditInformation(long billId)
        {
            var response = await clientForLoan.GetCreditByBillIdAsync(new CreditBillIdModel
            {
                Id = Convert.ToInt64(billId)
            });

            var tariff = new CreditViewModel()
            {
                BillId = response.BillId,
                Dept = response.Dept,
                Duration = response.Duration,
                Id = response.Id,
                InitialSum = response.InitialSum,
                RemainingSum = response.RemainingSum,
                LatePaymentModels = new List<ViewModels.LatePaymentModel>(),
                TariffInfoModel = new ViewModels.TariffInfoModel()
                {
                    Id = response.Tariff.Id,
                    Name = response.Tariff.Name,
                    Percentage = response.Tariff.Percentage
                }
            };

            foreach (var latePaymentModel in response.LatePayments)
            {
                tariff.LatePaymentModels.Add(new ViewModels.LatePaymentModel
                {
                    Date = latePaymentModel.Date,
                    Amount = latePaymentModel.Amount
                });
            }

            return tariff;
        }
    }
}