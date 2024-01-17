using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Users;

namespace Golbaus_BE.Services.Interface
{
	public interface IAccountService
	{
		Task<ErrorModel> CreateUser(CreateUserModel model, string hostName);
		UserGetByTokenModel GetByToken();
	}
}
