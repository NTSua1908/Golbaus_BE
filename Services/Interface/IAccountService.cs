using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Users;

namespace Golbaus_BE.Services.Interface
{
	public interface IAccountService
	{
		Task<ErrorModel> CreateUser(CreateUserModel model);
		UserGetByTokenModel GetByToken();
		GetDetailModel GetDetailByToken(ErrorModel errors);
		GetUserProfileModel GetDetailById(string userId, ErrorModel errors);
		void UpdateByToken(UserUpdateByTokenModel model, ErrorModel errors);
		void UpdateAvatarByToken(UpdateAvatarModel model, ErrorModel errors);
		void ToggleFollow(string userId, ErrorModel errors);
	}
}
