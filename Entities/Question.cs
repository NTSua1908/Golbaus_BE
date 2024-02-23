using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities.BaseEntity;

namespace Golbaus_BE.Entities
{
    public class Question
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; }
		public string UpdatedBy{ get; set; }
		public string UserId { get; set; }
		public virtual User User{ get; set; }
		public virtual NewestQuestion NewestQuestion { get; set; }
		public virtual ICollection<CommentQuestion> CommentQuestions { get; set; }
		public virtual ICollection<QuestionTagMap> QuestionTagMaps { get; set; }
		public virtual ICollection<QuestionUserVoteMap> QuestionUserVoteMaps { get; set; }
		public virtual ICollection<QuestionBookmark> QuestionBookmarks { get; set; }
	}
}
