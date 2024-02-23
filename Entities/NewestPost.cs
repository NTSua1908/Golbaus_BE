using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golbaus_BE.Entities
{
	public class NewestPost
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public DateTime PublishDate { get; set; }
        public virtual Post Post { get; set; }
    }
}
