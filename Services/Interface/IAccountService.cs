using Golbaus_BE.DTOs.Users;

namespace Golbaus_BE.Services.Interface
{
	public interface IAccountService
	{
		UserGetByTokenModel GetByToken();
	}
}
