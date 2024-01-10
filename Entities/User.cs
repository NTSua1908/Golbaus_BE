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
        public string Avatar { get; set; }
        public string Bio { get; set; }
        public int PostCount { get; set; }
        public virtual ICollection<UserRoleMap> UserRoleMaps { get; set; }
		public virtual ICollection<Post> Posts { get; set; }
		public virtual ICollection<Comment> Comments { get; set; }
		public virtual ICollection<UserFollowMap> UserFollowerMaps{ get; set; }
		public virtual ICollection<UserFollowMap> UserFollowingMaps{ get; set; }
		public virtual ICollection<PostUserVoteMap> PostUserVoteMaps { get; set; }
		public virtual ICollection<CommentUserVoteMap> CommentUserVoteMaps { get; set; }
	}
}
