using Microsoft.AspNetCore.Identity;

namespace Golbaus_BE.Entities
{
	public class Role : IdentityRole
	{
		public virtual ICollection<UserRoleMap> UserRoleMaps { get; set; }
	}
}
