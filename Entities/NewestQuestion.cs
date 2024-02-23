using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Golbaus_BE.Entities
{
	public class NewestQuestion
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		public Guid QuestionId { get; set; }
		public DateTime CreatedDate { get; set; }
		public virtual Question Question { get; set; }
	}
}
