using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Golbaus_BE.Entities
{
	public class Comment
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
        public string Content { get; set; }
        public string ReplyFor { get; set; }
		public int UpVote { get; set; }
		public int DownVote { get; set; }
		public string Remark { get; set; }
		public bool IsDeleted { get; set; }
        public Guid? ParentId { get; set; }
        public Comment Parent { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public string UserId { get; set; }
        public virtual User User { get; set; }
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
		public virtual ICollection<Comment> Childs { get; set; }
    }
}
