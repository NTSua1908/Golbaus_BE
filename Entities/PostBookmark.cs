using Microsoft.AspNetCore.Identity;

namespace Golbaus_BE.Entities
{
	public class PostBookmark
	{
		public string UserId { get; set; }
		public Guid PostId { get; set; }
        public DateTime MarkedDate { get; set; }
        public virtual User User { get; set; }
		public virtual Post Post { get; set; }
	}
}
