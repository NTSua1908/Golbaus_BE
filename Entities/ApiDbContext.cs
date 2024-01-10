using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Golbaus_BE.Extentions;

namespace Golbaus_BE.Entities
{
	public class ApiDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRoleMap , IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
	{
		public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

		public static ApiDbContext Create(DbContextOptions<ApiDbContext> options)
		{
			return new ApiDbContext(options);
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			// Customize the ASP.NET Identity model and override the defaults if needed.
			// For example, you can rename the ASP.NET Identity table names and more.
			// Add your customizations after calling base.OnModelCreating(builder);
			base.OnModelCreating(builder);

			//Configuration relationship
			builder.ConfigDefaultDB();
			//Seed
			builder.Seed();
		}
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<UserRoleMap> UserRoleMaps { get; set; }
		public DbSet<Post> Posts{ get; set; }
		public DbSet<Comment> Comments{ get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<PostTagMap> PostTagMaps { get; set; }
		public DbSet<UserFollowMap> UserFollowMaps { get; set; }
		public DbSet<PostUserVoteMap> PostUserVoteMaps { get; set; }
		public DbSet<CommentUserVoteMap> CommentUserVoteMaps { get; set; }
	}
}
