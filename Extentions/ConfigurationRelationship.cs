using Golbaus_BE.Entities;
using Microsoft.EntityFrameworkCore;

namespace Golbaus_BE.Extentions
{
	public static class ConfigurationRelationship
	{
		public static void ConfigDefaultDB(this ModelBuilder builder)
		{
			builder.Entity<UserRoleMap>(userRole =>
			{
				userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

				userRole.HasOne(ur => ur.Role)
					.WithMany(r => r.UserRoleMaps)
					.HasForeignKey(ur => ur.RoleId)
					.IsRequired();

				userRole.HasOne(ur => ur.User)
					.WithMany(r => r.UserRoleMaps)
					.HasForeignKey(ur => ur.UserId)
					.IsRequired();
			});

			builder.Entity<User>(user =>
			{
				user.HasIndex(x => x.Email).IsUnique();
			});
		}
	}
}
