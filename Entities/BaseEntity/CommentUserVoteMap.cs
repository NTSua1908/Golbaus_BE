using Golbaus_BE.Commons.Constants;

namespace Golbaus_BE.Entities.BaseEntity
{
    public class CommentUserVoteMap
    {
        public string UserId { get; set; }
        public VoteType Type { get; set; }
        public virtual User User { get; set; }
    }
}
