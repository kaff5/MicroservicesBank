using EmployeeClient.ViewModels;
using Grpc.Net.Client;
using LoanService;
using UsersService;

namespace EmployeeClient.Services
{
    public interface IEmployeeService
    {
        Task<UserViewModel> RegisterClient(RegisterUserViewModel model);
        Task<UserViewModel> RegisterEmployee(RegisterUserViewModel model);
		Task CreateTariff(CreateTariffViewModel model);
	}
    public class EmployeeService: IEmployeeService
    {
        private readonly Users.UsersClient client;
		private readonly Loaner.LoanerClient clientForLoan;

		public EmployeeService()
        {
			var portUsersService = Environment.GetEnvironmentVariable("PortUsersService");
			var ipUsersService = Environment.GetEnvironmentVariable("IpUsersService");
            var channel = GrpcChannel.ForAddress($"http://{ipUsersService}:{portUsersService}");
			client = new Users.UsersClient(channel);
        }

        public async Task<UserViewModel> RegisterEmployee(RegisterUserViewModel model)
        {
            var response = await client.RegisterNewEmployeeAsync(
                new RegisterUserRequest()
                {
                    Name = model.Name,
                    Password = model.Password,
                    Patronymic = model.Patronymic,
                    Surname = model.Surname,
                    UserName = model.Username
                }
            );

			return new UserViewModel
			{
				UserId = response.Id
			};
        }
        public async Task<UserViewModel> RegisterClient(RegisterUserViewModel model)
        {
            var response = await client.RegisterNewClientAsync(new RegisterUserRequest
            {
                Name = model.Name,
                Password = model.Password,
                Patronymic = model.Patronymic,
                Surname = model.Surname,
                UserName = model.Username
            });

			return new UserViewModel
			{
				UserId = response.Id
			};
		}

		public async Task CreateTariff(CreateTariffViewModel model)
		{
			var response = await clientForLoan.CreateTariffAsync(new CreateTariffModel
			{
				Name = model.Name,
				Percentage = model.Percentage,
			});
		}
	}
}
