using Golbaus_BE.Commons.Constants;

namespace Golbaus_BE.Entities
{
	public class PostUserVoteMap
	{
		public string UserId { get; set; }
		public Guid PostId { get; set; }
		public VoteType Type { get; set; }
		public virtual User User { get; set; }
		public virtual Post Post { get; set; }
	}
}
