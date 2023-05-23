using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersService.Data;
using UsersService.Models;

namespace UsersService.Services
{
    public class UserService : Users.UsersBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UserService(ApplicationDbContext dbcontext, UserManager<User> userManager)
        {
            _dbContext = dbcontext;
            _userManager = userManager;
        }

        public override async Task<UserReply> GetUserById(UserRequestById request, ServerCallContext context)
        {
            var parseId = Guid.Parse(request.UserId);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == parseId);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found user"));
            }

            var role = await _userManager.GetRolesAsync(user);
            if (role == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found role user"));
            }

            if (role.Any())
            {
                var reply = new UserReply()
                {
                    UserId = user.Id.ToString(),
                    Name = user.Name,
                    Patronymic = user.Patronymic,
                    Surname = user.Surname,
                    Role = role.FirstOrDefault(),
                    UserName = user.Name,
                    IsBlocked = user.isBlocked
                };

                return reply;
            }

            throw new RpcException(new Status(StatusCode.NotFound, "Not found role user"));
        }

        public override async Task<ListUsers> GetAllUsers(GetAllUsersRequest request, ServerCallContext context)
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found user"));
            }

            var reply = new ListUsers();

            foreach (var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);
                if (role == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Not found role user"));
                }

                reply.Users.Add(new UserReply
                {
                    UserId = user.Id.ToString(),
                    Name = user.Name,
                    Patronymic = user.Patronymic,
                    Surname = user.Surname,
                    Role = role.FirstOrDefault(),
                    UserName = user.Name,
                    IsBlocked = user.isBlocked
                });
            }


            return await Task.FromResult(reply);
        }

        public override async Task<SuccessfullyReply> BlockUserById(UserRequestById request, ServerCallContext context)
        {
            var parseId = Guid.Parse(request.UserId);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == parseId);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found user"));
            }

            user.isBlocked = true;

            await _dbContext.SaveChangesAsync();

            return new SuccessfullyReply
            {
                Message = "Successfully"
            };
        }

        public override async Task<RegisterUserReply> RegisterNewEmployee(RegisterUserRequest request,
            ServerCallContext context)
        {
            var name = request.Name;
            var surname = request.Surname;
            var patronymic = request.Patronymic;
            var userName = request.UserName;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(patronymic) || string.IsNullOrWhiteSpace(userName))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad registration data"));
            }

            var employee = new User()
            {
                Name = name,
                Surname = surname,
                Patronymic = patronymic,
                UserName = userName,
                CreateAt = DateTime.UtcNow
            };


            await _userManager.CreateAsync(employee, request.Password);
            await _userManager.AddToRoleAsync(employee, "Employee");
            await _dbContext.SaveChangesAsync();

            return new RegisterUserReply
            {
                Name = employee.Name,
                Surname = employee.Surname,
                Patronymic = employee.Patronymic,
                UserName = employee.UserName,
                Id = employee.Id.ToString(),
                IsBlocked = employee.isBlocked
            };
        }

        public override async Task<RegisterUserReply> RegisterNewClient(RegisterUserRequest request,
            ServerCallContext context)
        {
            var name = request.Name;
            var surname = request.Surname;
            var patronymic = request.Patronymic;
            var userName = request.UserName;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(patronymic) || string.IsNullOrWhiteSpace(userName))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Bad registration data"));
            }

            var client = new User()
            {
                Name = name,
                Surname = surname,
                Patronymic = patronymic,
                UserName = userName,
                CreateAt = DateTime.UtcNow
            };

            await _userManager.CreateAsync(client, request.Password);
            await _userManager.AddToRoleAsync(client, "Client");
            await _dbContext.SaveChangesAsync();

            return new RegisterUserReply
            {
                Name = client.Name,
                Surname = client.Surname,
                Patronymic = client.Patronymic,
                UserName = client.UserName,
                Id = client.Id.ToString(),
                IsBlocked = client.isBlocked
            };
        }
    }
}