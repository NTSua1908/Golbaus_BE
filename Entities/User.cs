using Golbaus_BE.Commons.Constants;
using Microsoft.AspNetCore.Identity;

namespace Golbaus_BE.Entities
{
	public class User : IdentityUser
	{
		public string FullName { get; set; }
		public DateTime DateJoined { get; set; }
		public DateTime? DoB { get; set; }
        public Gender Gender { get; set; }
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
		public virtual ICollection<UserRoleMap> UserRoleMaps { get; set; }
	}
}
