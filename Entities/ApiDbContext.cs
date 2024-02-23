using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Golbaus_BE.Extentions;
using Golbaus_BE.Entities.BaseEntity;

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
		public DbSet<Question> Questions{ get; set; }
		public DbSet<CommentPost> CommentPosts{ get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<PostTagMap> PostTagMaps { get; set; }
		public DbSet<UserFollowMap> UserFollowMaps { get; set; }
		public DbSet<PostUserVoteMap> PostUserVoteMaps { get; set; }
		public DbSet<CommentPostUserVoteMap> CommentPostUserVoteMaps { get; set; }
		public DbSet<QuestionTagMap> QuestionTagMaps{ get; set; }
		public DbSet<QuestionUserVoteMap> QuestionUserVoteMaps{ get; set; }
		public DbSet<CommentQuestionUserVoteMap> CommentQuestionUserVoteMaps{ get; set; }
		public DbSet<CommentQuestion> CommentQuestions { get; set; }
		public DbSet<PostBookmark> PostBookmarks { get; set; }
		public DbSet<QuestionBookmark> QuestionBookmarks { get; set; }
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<NewestPost> NewestPosts{ get; set; }
		public DbSet<TrendingPost> TrendingPosts { get; set; }
		public DbSet<NewestQuestion> NewestQuestions { get; set; }
	}
}
