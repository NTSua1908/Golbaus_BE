namespace Golbaus_BE.Entities
{
	public class UserFollowMap
	{
        public string FollowerId { get; set; }
        public string FollowingId { get; set; }
        public virtual User Follower { get; set; }
        public virtual User Following { get; set; }
    }
}
