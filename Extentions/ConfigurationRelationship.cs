using Golbaus_BE.Entities;
using Golbaus_BE.Entities.BaseEntity;
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

			builder.Entity<Post>(post =>
			{
				post.HasOne(x => x.User)
					.WithMany(user => user.Posts)
					.HasForeignKey(x => x.UserId)
					.IsRequired();
			});

			builder.Entity<CommentPost>(comment =>
			{
				comment.HasOne(x => x.User)
						.WithMany(user => user.CommentPosts)
						.IsRequired();
				comment.HasOne(x => x.Post)
						.WithMany(post => post.CommentPosts)
						.IsRequired();
				comment.HasOne(x => x.Parent)
						.WithMany(x => x.Childs)
						.HasForeignKey(x => x.ParentId)
						.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<Tag>(tag =>
			{
				tag.HasIndex(x => x.Name)
					.IsUnique();
			});

			builder.Entity<PostTagMap>(postTagMap =>
			{
				postTagMap.HasKey(x => new { x.PostId, x.TagId });
				postTagMap.HasOne(x => x.Post)
						.WithMany(post => post.PostTagMaps)
						.HasForeignKey(x => x.PostId)
						.IsRequired();
				postTagMap.HasOne(x => x.Tag)
						.WithMany(tag => tag.PostTagMaps)
						.HasForeignKey(x => x.TagId)
						.IsRequired();
			});

			builder.Entity<UserFollowMap>(followMap=>
			{
				followMap.HasKey(x => new { x.FollowerId, x.FollowingId});
				followMap.HasOne(x => x.Follower)
						.WithMany(user  => user.UserFollowerMaps)
						.HasForeignKey(x => x.FollowerId)
						.IsRequired();
				followMap.HasOne(x => x.Following)
						.WithMany(user  => user.UserFollowingMaps)
						.HasForeignKey(x => x.FollowingId)
						.IsRequired();
			});

			builder.Entity<PostUserVoteMap>(voteMap =>
			{
				voteMap.HasKey(x => new { x.UserId, x.PostId});
				voteMap.HasOne(x => x.User)
						.WithMany(user => user.PostUserVoteMaps)
						.HasForeignKey(x => x.UserId)
						.IsRequired();
				voteMap.HasOne(x => x.Post)
						.WithMany(post => post.PostUserVoteMaps)
						.HasForeignKey(x => x.PostId)
						.IsRequired();
			});

			builder.Entity<CommentPostUserVoteMap>(voteMap =>
			{
				voteMap.HasKey(x => new { x.UserId, x.CommentId });
				voteMap.HasOne(x => x.User)
						.WithMany(user => user.CommentPostUserVoteMaps)
						.HasForeignKey(x => x.UserId)
						.IsRequired();
				voteMap.HasOne(x => x.Comment)
						.WithMany(comment => comment.CommentPostUserVoteMaps)
						.HasForeignKey(x => x.CommentId)
						.IsRequired();
			});

			builder.Entity<Question>(question=>
			{
				question.HasOne(x => x.User)
					.WithMany(user => user.Questions)
					.HasForeignKey(x => x.UserId)
					.IsRequired();
			});

			builder.Entity<CommentQuestion>(comment =>
			{
				comment.HasOne(x => x.User)
						.WithMany(user => user.CommentQuestions)
						.IsRequired();
				comment.HasOne(x => x.Question)
						.WithMany(question => question.CommentQuestions)
						.IsRequired();
				comment.HasOne(x => x.Parent)
						.WithMany(x => x.Childs)
						.HasForeignKey(x => x.ParentId)
						.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<QuestionTagMap>(QuestionTagMap =>
			{
				QuestionTagMap.HasKey(x => new { x.QuestionId, x.TagId });
				QuestionTagMap.HasOne(x => x.Question)
						.WithMany(question => question.QuestionTagMaps)
						.HasForeignKey(x => x.QuestionId)
						.IsRequired();
				QuestionTagMap.HasOne(x => x.Tag)
						.WithMany(tag => tag.QuestionTagMaps)
						.HasForeignKey(x => x.TagId)
						.IsRequired();
			});

			builder.Entity<QuestionUserVoteMap>(voteMap =>
			{
				voteMap.HasKey(x => new { x.UserId, x.QuestionId });
				voteMap.HasOne(x => x.User)
						.WithMany(user => user.QuestionUserVoteMaps)
						.HasForeignKey(x => x.UserId)
						.IsRequired();
				voteMap.HasOne(x => x.Question)
						.WithMany(question => question.QuestionUserVoteMaps)
						.HasForeignKey(x => x.QuestionId)
						.IsRequired();
			});

			builder.Entity<CommentQuestionUserVoteMap>(voteMap =>
			{
				voteMap.HasKey(x => new { x.UserId, x.CommentId });
				voteMap.HasOne(x => x.User)
						.WithMany(user => user.CommentQuestionUserVoteMaps)
						.HasForeignKey(x => x.UserId)
						.IsRequired();
				voteMap.HasOne(x => x.Comment)
						.WithMany(comment => comment.CommentQuestionUserVoteMaps)
						.HasForeignKey(x => x.CommentId)
						.IsRequired();
			});
		}
	}
}
