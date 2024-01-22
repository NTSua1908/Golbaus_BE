using Golbaus_BE.Commons.Constants;

namespace Golbaus_BE.Entities
{
	public class QuestionUserVoteMap
	{
		public string UserId { get; set; }
		public Guid QuestionId { get; set; }
		public VoteType Type { get; set; }
		public virtual User User { get; set; }
		public virtual Question Question { get; set; }
	}
}
