using Golbaus_BE.Entities.BaseEntity;

namespace Golbaus_BE.Entities
{
	public class CommentQuestion : Comment
	{
		public Guid? ParentId { get; set; }
		public CommentQuestion Parent { get; set; }
		public virtual ICollection<CommentQuestion> Childs { get; set; }
		public Guid QuestionId { get; set; }
		public virtual Question Question { get; set; }
		public virtual ICollection<CommentQuestionUserVoteMap> CommentQuestionUserVoteMaps { get; set; }
	}
}
