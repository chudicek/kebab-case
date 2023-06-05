namespace Kebabos.Services.User;

using Kebabos.Models;
using Kebabos.Contracts.User;

public interface IUserService
{
    Task<ServiceResult<User>> CreateUser(UserCreateRequest request);
    Task<ServiceResult<List<User>>> GetUsers();
    Task<ServiceResult<User>> GetUserById(Guid id);
}