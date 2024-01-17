using Golbaus_BE.Commons.Helper;
using Golbaus_BE.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Golbaus_BE.Extentions
{
	public static class ModelBuilderExtensions
	{
		public static void Seed(this ModelBuilder modelBuilder)
		{
			var roleIdSuperAdmin = new string("90eff107-f2ce-4068-9568-30edacbc8214");
			var roleIdAdmin = new string("8bfb5745-ba0f-4011-aace-6e75e8502cdf");
			var roleIdUser = new string("0d37bd4f-6550-4dbc-8894-23b571194be4");
			modelBuilder.Entity<Role>().HasData(
				new Role()
				{
					Id = roleIdSuperAdmin,
					ConcurrencyStamp = "fa28fea7-add7-4636-9bb0-bd162d7ce2aa",
					Name = "SuperAdmin",
					NormalizedName = "SUPERADMIN"
				},
				new Role()
				{
					Id = roleIdAdmin,
					ConcurrencyStamp = "bb25e5b9-8212-409f-8196-54408a0904f2",
					Name = "Admin",
					NormalizedName = "Admin"
				},
				new Role()
				{
					Id = roleIdUser,
					ConcurrencyStamp = "f1fe13ac-2edc-4e98-9272-3bce3b6c4240",
					Name = "User",
					NormalizedName = "User",
				}
			);
			var hasher = new PasswordHasher<User>();

			var adminId = new string("4bfcc5f7-5bdc-4827-a909-4f04ac5770ff");
			var superAdminId = new string("0f82194c-dc6a-45ff-8ad2-5a6ea82be10f");
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = adminId,
					FullName = DefaultAdmin.FullName,
					UserName = DefaultAdmin.UserName,
					NormalizedUserName = DefaultAdmin.UserName.ToUpper(),
					Email = DefaultAdmin.Email,
					NormalizedEmail = DefaultAdmin.Email.ToUpper(),
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, DefaultAdmin.Password),
					SecurityStamp = string.Empty,
					ConcurrencyStamp = "71229555-966a-4b46-ba94-06a26b8b0fd6",
				},
				new User
				{
					Id = superAdminId,
					FullName = DefaultSuperAdmin.FullName,
					UserName = DefaultSuperAdmin.UserName,
					NormalizedUserName = DefaultSuperAdmin.UserName.ToUpper(),
					Email = DefaultSuperAdmin.Email,
					NormalizedEmail = DefaultSuperAdmin.Email.ToUpper(),
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, DefaultSuperAdmin.Password),
					SecurityStamp = string.Empty,
					ConcurrencyStamp = "0df00d6b-0cdc-4eaf-948f-d3cdbbe92b44",
				}
			);

			modelBuilder.Entity<UserRoleMap>().HasData(
				new UserRoleMap
				{
					RoleId = roleIdSuperAdmin,
					UserId = superAdminId
				},
				new UserRoleMap
				{
					RoleId = roleIdAdmin,
					UserId = adminId
				}
			);
		}
	}
}
