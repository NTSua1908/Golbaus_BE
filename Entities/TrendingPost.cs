using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golbaus_BE.Entities
{
	public class TrendingPost
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int ViewCount { get; set; }
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
