using Microsoft.AspNetCore.Identity;

namespace Golbaus_BE.Entities
{
	public class UserRoleMap : IdentityUserRole<string>
	{
		public virtual User User { get; set; }
		public virtual Role Role { get; set; }
	}
}
