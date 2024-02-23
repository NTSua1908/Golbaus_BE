using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities.BaseEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Golbaus_BE.Entities
{
    public class User : IdentityUser
	{

		public string FullName { get; set; }
		public DateTime DateJoined { get; set; }
		public DateTime? DoB { get; set; }
        public Gender Gender { get; set; }
        public string Avatar { get; set; }
        public string Bio { get; set; }
		public bool IsDeleted { get; set; }
		public string RecentlyViewedTags { get; set; } = "";
        public virtual ICollection<UserRoleMap> UserRoleMaps { get; set; }
		public virtual ICollection<Post> Posts { get; set; }
		public virtual ICollection<Question> Questions{ get; set; }
		public virtual ICollection<CommentPost> CommentPosts { get; set; }
		public virtual ICollection<CommentQuestion> CommentQuestions { get; set; }
		public virtual ICollection<CommentPostUserVoteMap> CommentPostUserVoteMaps { get; set; }
		public virtual ICollection<CommentQuestionUserVoteMap> CommentQuestionUserVoteMaps { get; set; }
		public virtual ICollection<PostUserVoteMap> PostUserVoteMaps { get; set; }
		public virtual ICollection<QuestionUserVoteMap> QuestionUserVoteMaps { get; set; }
		public virtual ICollection<UserFollowMap> UserFollowerMaps{ get; set; }
		public virtual ICollection<UserFollowMap> UserFollowingMaps{ get; set; }
		public virtual ICollection<PostBookmark> PostBookmarks { get; set; }
		public virtual ICollection<QuestionBookmark> QuestionBookmarks { get; set; }
		public virtual ICollection<Notification> Notifications { get; set; }
		public virtual ICollection<Notification> NotificationInvolves { get; set; }
	}
}
