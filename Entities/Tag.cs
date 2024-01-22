using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Golbaus_BE.Entities
{
	public class Tag
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
        public string Name { get; set; }
		public virtual ICollection<PostTagMap> PostTagMaps { get; set; }
		public virtual ICollection<QuestionTagMap> QuestionTagMaps { get; set; }
	}
}
