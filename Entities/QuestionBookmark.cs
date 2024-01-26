using Microsoft.AspNetCore.Identity;

namespace Golbaus_BE.Entities
{
	public class QuestionBookmark 
	{
        public string UserId { get; set; }
        public Guid QuestionId { get; set; }
        public virtual User User { get; set; }
		public virtual Question Question { get; set; }
	}
}
