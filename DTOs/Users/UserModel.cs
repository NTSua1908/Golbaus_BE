using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities;
using Role = Golbaus_BE.Commons.Constants.Role;

namespace Golbaus_BE.DTOs.Users
{
	public class UserGetByTokenModel
	{
		public string FullName { get; set; }
		public string? Avatar { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }

		public UserGetByTokenModel(User user) {
			UserName = user.UserName;
			Avatar = user.Avatar;
			FullName = user.FullName;
			Role = Enum.Parse<Role>(user.UserRoleMaps.First().Role.Name);
		}
    }
}
