using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Role = Golbaus_BE.Commons.Constants.Role;

namespace Golbaus_BE.DTOs.Users
{
	public class UserGetByTokenModel
	{
        public string Id { get; set; }
        public string FullName { get; set; }
		public string? Avatar { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }

		public UserGetByTokenModel(User user) 
		{
			Id = user.Id;
			FullName = user.FullName;
			UserName = user.UserName;
			Avatar = user.Avatar;
			Role = Enum.Parse<Role>(user.UserRoleMaps.First().Role.Name);
		}
    }

	public class CreateUserModel
	{
		[Required(ErrorMessage = "FullName is required")]
		[MinLength(3, ErrorMessage = "FullName must have at least 3 characters"), MaxLength(20, ErrorMessage = "FullName can have a maximum of 20 characters")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "UserName is required")]
		[MinLength(3, ErrorMessage = "UserName must have at least 3 characters"), MaxLength(15, ErrorMessage = "UserName can have a maximum of 15 characters")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Must be a valid email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "Invalid password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "ConfirmPassword is required")]
		public string ConfirmPassword { get; set; }

		public User ParseToEntity(string password)
		{
			return new User
			{
				FullName = FullName,
				UserName = UserName,
				Email = Email,
				DateJoined = DateTime.Now,
				EmailConfirmed = false,
				PasswordHash = password,
				NormalizedUserName = Email.ToUpper()
			};
		}
    }
}
