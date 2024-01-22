using Golbaus_BE.Entities.BaseEntity;

namespace Golbaus_BE.Entities
{
	public class CommentQuestionUserVoteMap : CommentUserVoteMap
	{
		public Guid CommentId { get; set; }
		public virtual CommentQuestion Comment { get; set; }
	}
}
