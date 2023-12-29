namespace Golbaus_BE.Entities
{
	public class PostTagMap
	{
        public Guid PostId { get; set; }
        public Guid TagId { get; set; }
        public virtual Post Post { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
