using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities.BaseEntity;

namespace Golbaus_BE.Entities
{
    public class Post
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
        public string Title { get; set; }
        public string Excerpt { get; set; }
        public string Thumbnail { get; set; }
        public string Content { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }
        public int ViewCount { get; set; }
		public PublishType PublishType { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; }
		public string UpdatedBy{ get; set; }
		public string UserId { get; set; }
		public virtual User User{ get; set; }
		public virtual ICollection<CommentPost> CommentPosts { get; set; }
		public virtual ICollection<PostTagMap> PostTagMaps{ get; set; }
		public virtual ICollection<PostUserVoteMap> PostUserVoteMaps { get; set; }
	}
}
