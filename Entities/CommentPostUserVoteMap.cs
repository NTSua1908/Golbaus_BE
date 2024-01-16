using Golbaus_BE.Entities.BaseEntity;

namespace Golbaus_BE.Entities
{
	public class CommentPostUserVoteMap : CommentUserVoteMap
	{
		public Guid CommentId { get; set; }
		public virtual CommentPost Comment { get; set; }
	}
}
