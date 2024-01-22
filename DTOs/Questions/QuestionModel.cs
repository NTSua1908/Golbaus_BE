using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities;
using System.ComponentModel.DataAnnotations;

namespace Golbaus_BE.DTOs.Questions
{
	public class QuestionCreateUpdateModel
	{
		[Required(ErrorMessage = "Title is required")]
		public string Title { get; set; }
		[Required(ErrorMessage = "Content is required")]
		public string Content { get; set; }
		[Required(ErrorMessage = "Tags is required")]
		public List<string> Tags { get; set; }

		public Question ParseToEntity(string userId, List<string> newTags, List<Tag> existedTags)
		{
			List<QuestionTagMap> questionTags = CreateQuestionTagMaps(newTags, existedTags);
			return new Question
			{
				Title = Title,
				Content = Content,
				QuestionTagMaps = questionTags,
				CreatedDate = DateTime.Now,
				UserId = userId,
			};
		}

		public void UpdateEntity(Question question, List<string> newTags, List<Tag> existedTags, string userId)
		{
			question.Title = Title;
			question.Content = Content;
			question.UpdatedDate = DateTime.Now;
			question.QuestionTagMaps = CreateQuestionTagMaps(newTags, existedTags);
			question.UpdatedBy = userId;
		}

		private List<QuestionTagMap> CreateQuestionTagMaps(List<string> newTags, List<Tag> existedTags)
		{
			List<QuestionTagMap> postTags = new List<QuestionTagMap>();
			newTags.ForEach(tag =>
			{
				postTags.Add(new QuestionTagMap
				{
					Tag = new Tag { Name = tag }
				});
			});
			existedTags.ForEach(tag =>
			{
				postTags.Add(new QuestionTagMap
				{
					Tag = tag
				});
			});
			return postTags;
		}
	}

	public class QuestionDetailModel
	{
        public Guid Id { get; set; }
        public string Title { get; set; }
		public string Content { get; set; }
		public int VoteCount { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public DateTime CreatedDate { get; set; }
		public string FullName { get; set; }
		public string UserName { get; set; }
		public string Avatar { get; set; }
		public bool IsFollowed { get; set; }
		public bool IsMyQuestion { get; set; }
		public int QuestionCount { get; set; }
		public int FollowCount { get; set; }
		public int AnswerCount { get; set; }
		public int ViewCount { get; set; }
		public VoteType? Vote { get; set; }
		public List<string> Tags { get; set; }

		public QuestionDetailModel() { }

		public QuestionDetailModel(Question question, string viewerId)
		{
			Id = question.Id;
			Title = question.Title;
			Content = question.Content;
			VoteCount = question.UpVote - question.DownVote;
			FullName = question.User.FullName;
			UserName = question.User.UserName;
			Avatar = question.User.Avatar;
			IsFollowed = question.User.UserFollowerMaps.Any(x => x.FollowerId == viewerId);
			IsMyQuestion = viewerId == question.UserId;
			QuestionCount = question.User.Questions.Count;
			FollowCount = question.User.UserFollowerMaps.Count();
			AnswerCount = question.CommentQuestions.Count(x => !x.IsDeleted);
			ViewCount = question.ViewCount;
			var vote = question.QuestionUserVoteMaps.FirstOrDefault(x => x.QuestionId == question.Id && x.UserId == viewerId);
			Vote = vote == null ? VoteType.Unvote : vote.Type;
			Tags = question.QuestionTagMaps.Select(x => x.Tag.Name).ToList();
			CreatedDate = question.CreatedDate;
			UpdatedDate = question.UpdatedDate;
		}
	}

	public class QuestionListModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string UserName { get; set; }
		public string Avatar { get; set; }
		public int AnswerCount { get; set; }
		public int ViewCount { get; set; }
		public int FollowCount { get; set; }
		public List<string> Tags { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public DateTime CreatedDate { get; set; }

		public QuestionListModel(Question question)
		{
			Id = question.Id;
			Title = question.Title;
			UserName = question.User.UserName;
			Avatar = question.User.Avatar;
			FollowCount = question.User.UserFollowerMaps.Count();
			AnswerCount = question.CommentQuestions.Count(x => !x.IsDeleted);
			ViewCount = question.ViewCount;
			Tags = question.QuestionTagMaps.Select(x => x.Tag.Name).ToList();
			CreatedDate = question.CreatedDate;
			UpdatedDate = question.UpdatedDate;
		}
	}
}
