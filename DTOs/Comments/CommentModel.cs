using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities;
using Golbaus_BE.Entities.BaseEntity;

namespace Golbaus_BE.DTOs.Comments
{
	public class CommentCreateModel
	{
		public string Content { get; set; }
		public string ReplyFor { get; set; }
		public Guid? ParentId { get; set; }
		public Guid PostId { get; set; }

		public CommentPost ParseToPostCommentEntity(Post post, CommentPost parent, string userId)
		{
			return new CommentPost
			{
				Content = Content,
				ParentId = parent?.Id,
				PostId = post.Id,
				CreatedDate = DateTime.Now,
				Remark = "",
				ReplyFor = ReplyFor,
				UserId = userId
			};
		}
	}

	public class CommentUpdateModel
	{
        public string Content { get; set; }

		public void UpdateEntity(Comment comment)
		{
			comment.Content = Content;
		}
    }

	public class CommentDetailModel
	{
		public Guid Id { get; set; }
		public string Avatar { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string Content { get; set; }
		public string ReplyFor { get; set; }
		public int UpVote { get; set; }
		public int DownVote { get; set; }
		public int ReplyCount { get; set; }
        public VoteType VoteType { get; set; }
        public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
        public bool IsMyComment { get; set; }
        public List<CommentDetailModel> Replies { get; set; }


		//Return data for create new comment
        public CommentDetailModel(Comment comment, User user)
		{
			Id = comment.Id;
			Avatar = user.Avatar;
			FullName = user.FullName;
			UserName = user.UserName;
			Content = comment.Content; 
			ReplyFor = comment.ReplyFor; 
			UpVote = comment.UpVote;
			VoteType = VoteType.Unvote;
			ReplyCount = 0;
			DownVote = comment.DownVote;
			CreatedDate = comment.CreatedDate; 
			UpdatedDate = comment.UpdatedDate;
			IsMyComment = true;
			Replies = new List<CommentDetailModel>();
		}

		//Return data for list comment
		public CommentDetailModel(CommentPost comment, string userId, bool isChild)
		{
			Id = comment.Id;
			Avatar = comment.User.Avatar;
			FullName = comment.User.FullName;
			UserName = comment.User.UserName;
			Content = comment.Content;
			ReplyFor = comment.ReplyFor;
			ReplyCount = isChild ? 0 : comment.Childs.Count(x => !x.IsDeleted);
			UpVote = comment.UpVote;
			DownVote = comment.DownVote;
			var vote = comment.CommentPostUserVoteMaps.FirstOrDefault(x => x.UserId == userId);
			VoteType = vote != null ? vote.Type : VoteType.Unvote;
			CreatedDate = comment.CreatedDate;
			UpdatedDate = comment.UpdatedDate;
			IsMyComment = comment.UserId == userId;
			Replies = new List<CommentDetailModel>();
		}

		public CommentDetailModel() { }
	}
}
