using Golbaus_BE.Entities.BaseEntity;

namespace Golbaus_BE.Entities
{
	public class CommentPost : Comment
	{
		public Guid? ParentId { get; set; }
		public CommentPost Parent { get; set; }
		public virtual ICollection<CommentPost> Childs { get; set; }
		public Guid PostId { get; set; }
		public virtual Post Post { get; set; }
		public virtual ICollection<CommentPostUserVoteMap> CommentPostUserVoteMaps { get; set; }
	}
}
