using Golbaus_BE.Commons.Constants;

namespace Golbaus_BE.Entities
{
	public class CommentUserVoteMap
	{
		public string UserId { get; set; }
		public Guid CommentId { get; set; }
		public VoteType Type { get; set; }
		public virtual User User { get; set; }
		public virtual Comment Comment { get; set; }
	}
}
