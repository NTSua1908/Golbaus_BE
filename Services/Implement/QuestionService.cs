using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Questions;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace Golbaus_BE.Services.Implement
{
	public class QuestionService : IQuestionService
	{
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public QuestionService(ApiDbContext dbContext, UserResolverService userResolverService, IHttpContextAccessor httpContextAccessor)
		{
			_dbContext = dbContext;
			_userResolverService = userResolverService;
			_httpContextAccessor = httpContextAccessor;
		}

		public Guid Create(QuestionCreateUpdateModel model, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateCreateQuestion(userId, model, errors, out User user))
			{
				var newTags = GetTagNotExist(model.Tags, out List<Tag> existedTags);
				Question question = model.ParseToEntity(userId, newTags, existedTags);

				_dbContext.Questions.Add(question);
				_dbContext.SaveChanges();
				return question.Id;
			}
			return new Guid();
		}

		public void Update(Guid questionId, QuestionCreateUpdateModel model, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateUpdateQuestion(questionId, userId, model, errors, out Question question))
			{
				var newTags = GetTagNotExist(model.Tags, out List<Tag> existedTags);
				_dbContext.QuestionTagMaps.RemoveRange(question.QuestionTagMaps);
				model.UpdateEntity(question, newTags, existedTags, userId);
				_dbContext.SaveChanges();
			}
		}

		public QuestionDetailModel GetDetail(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			Question question = _dbContext.Questions.Include(x => x.QuestionTagMaps).ThenInclude(x => x.Tag)
										.Include(x => x.User).ThenInclude(x => x.UserFollowerMaps)
										.Include(x => x.User).ThenInclude(x => x.Questions)
										.Include(x => x.QuestionUserVoteMaps)
										.Include(x => x.CommentQuestions)
										.Include(x => x.QuestionBookmarks)
										.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else
			{
				_httpContextAccessor.HttpContext.Session.SetString("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				_httpContextAccessor.HttpContext.Session.SetString("QuestionId", id.ToString());
				Console.WriteLine(_httpContextAccessor.HttpContext.Session.Id);
				return new QuestionDetailModel(question, userId);
			}
			return new QuestionDetailModel();
		}

		public void Delete(Guid id, ErrorModel errors)
		{
			if (ValidateDeleteQuestion(id, errors, out Question question, out User user))
			{
				question.IsDeleted = true;
				question.UpdatedBy = user.Id;
				question.UpdatedDate = DateTime.Now;
				_dbContext.SaveChanges();
			}
		}

		public void DeleteByAdmin(Guid id, string remark, ErrorModel errors)
		{
			if (ValidateDeleteQuestionByAdmin(id, remark, errors, out Question question, out User user))
			{
				question.IsDeleted = true;
				question.Remark = remark;
				question.UpdatedBy = user.Id;
				question.UpdatedDate = DateTime.Now;
				_dbContext.SaveChanges();
			}
		}

		public void Restore(Guid id, ErrorModel errors)
		{
			if (ValidateRestoreQuestion(id, errors, out Question question, out User user))
			{
				question.IsDeleted = false;
				question.UpdatedBy = user.Id;
				question.UpdatedDate = DateTime.Now;
				_dbContext.SaveChanges();
			}
		}

		public void ToggleUpVote(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVoteQuestion(id, userId, errors, out Question question))
			{
				var voted = _dbContext.QuestionUserVoteMaps.FirstOrDefault(x => x.QuestionId == question.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.QuestionUserVoteMaps.Add(new QuestionUserVoteMap { QuestionId = question.Id, UserId = userId, Type = VoteType.UpVote });
					question.UpVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					voted.Type = VoteType.UpVote;
					question.UpVote += 1;
					question.DownVote -= 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					question.UpVote -= 1;
					_dbContext.QuestionUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		public void ToggleDownVote(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVoteQuestion(id, userId, errors, out Question question))
			{
				var voted = _dbContext.QuestionUserVoteMaps.FirstOrDefault(x => x.QuestionId == question.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.QuestionUserVoteMaps.Add(new QuestionUserVoteMap { QuestionId = question.Id, UserId = userId, Type = VoteType.DownVote });
					question.DownVote += 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					voted.Type = VoteType.DownVote;
					question.UpVote -= 1;
					question.DownVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					question.DownVote -= 1;
					_dbContext.QuestionUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		public void IncreaseView(Guid id, ErrorModel errors)
		{
			Question question = _dbContext.Questions.FirstOrDefault(x => x.Id == id);
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else
			{
				string timeGet = _httpContextAccessor.HttpContext.Session.GetString("Time");
				string questionId = _httpContextAccessor.HttpContext.Session.GetString("QuestionId");

				if (timeGet != null && questionId != null &&
					Guid.TryParse(questionId, out Guid QuestionId) && DateTime.TryParse(timeGet, out DateTime TimeGet) &&
					id == QuestionId && (DateTime.Now - TimeGet).TotalSeconds > 10)
				{
					question.ViewCount++;
					_dbContext.SaveChanges();
					_httpContextAccessor.HttpContext.Session.Remove("Time");
					_httpContextAccessor.HttpContext.Session.Remove("QuestionId");
				}
				else
				{
					errors.Add(string.Format(ErrorResource.Invalid, "Question"));
				}
			}
		}

		public PaginationModel<QuestionListModel> GetAll(PaginationPostQuestionRequest req)
		{
			var questions = _dbContext.Questions.Include(x => x.CommentQuestions).Include(x => x.User).ThenInclude(x => x.UserFollowerMaps)
										.Include(x => x.QuestionTagMaps).ThenInclude(x => x.Tag)
										.Where(x => !x.IsDeleted);
			Filter(req, ref questions);

			if (req.Tags != null)
			{
				var result = questions.AsEnumerable();
				req.Tags = req.Tags.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToList();
				result = result.Where(p => req.Tags.All(t => p.QuestionTagMaps.Any(m => m.Tag.Name.ToLower().Contains(t))));
				return new PaginationModel<QuestionListModel>(req, result.Select(x => new QuestionListModel(x)));
			}

			return new PaginationModel<QuestionListModel>(req, questions.Select(x => new QuestionListModel(x)));
		}

		public PaginationModel<QuestionListModel> GetAllByToken(PaginationPostQuestionRequest req)
		{
			string userId = _userResolverService.GetUser();
			var questions = _dbContext.Questions.Include(x => x.CommentQuestions).Include(x => x.User).ThenInclude(x => x.UserFollowerMaps)
										.Include(x => x.QuestionTagMaps).ThenInclude(x => x.Tag)
										.Where(x => x.UserId == userId && !x.IsDeleted);
			Filter(req, ref questions);

			if (req.Tags != null)
			{
				var result = questions.AsEnumerable();
				req.Tags = req.Tags.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToList();
				result = result.Where(p => req.Tags.All(t => p.QuestionTagMaps.Any(m => m.Tag.Name.ToLower().Contains(t))));
				return new PaginationModel<QuestionListModel>(req, result.Select(x => new QuestionListModel(x)));
			}

			return new PaginationModel<QuestionListModel>(req, questions.Select(x => new QuestionListModel(x)));
		}

		public PaginationModel<QuestionListModel> GetAllByUser(string userId, PaginationPostQuestionRequest req)
		{
			var questions = _dbContext.Questions.Include(x => x.CommentQuestions).Include(x => x.User).ThenInclude(x => x.UserFollowerMaps)
										.Include(x => x.QuestionTagMaps).ThenInclude(x => x.Tag)
										.Where(x => x.UserId == userId && !x.IsDeleted);
			Filter(req, ref questions);

			if (req.Tags != null)
			{
				var result = questions.AsEnumerable();
				req.Tags = req.Tags.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToList();
				result = result.Where(p => req.Tags.All(t => p.QuestionTagMaps.Any(m => m.Tag.Name.ToLower().Contains(t))));
				return new PaginationModel<QuestionListModel>(req, result.Select(x => new QuestionListModel(x)));
			}

			return new PaginationModel<QuestionListModel>(req, questions.Select(x => new QuestionListModel(x)));
		}

		public void ToggleAddBookmark(Guid id, ErrorModel errors)
		{
			Question question = _dbContext.Questions.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else
			{
				string userId = _userResolverService.GetUser();
				var mark = _dbContext.QuestionBookmarks.FirstOrDefault(x => x.UserId == userId && x.QuestionId == id);
				if (mark == null)
				{
					_dbContext.QuestionBookmarks.Add(new QuestionBookmark()
					{
						UserId = userId,
						QuestionId = id,
					});
				}
				else
				{
					_dbContext.QuestionBookmarks.Remove(mark);
				}
				_dbContext.SaveChanges();
			}
		}

		#region Helper

		private void Filter(PaginationPostQuestionRequest req, ref IQueryable<Question> data)
		{
			if (!string.IsNullOrEmpty(req.SearchText))
			{
				req.SearchText = req.SearchText.ToLower();
				data = data.Where(x => x.Title.ToLower().Contains(req.SearchText));
			}
			if (req.PublishDateFrom.HasValue)
			{
				data = data.Where(x => x.UpdatedDate.HasValue && x.UpdatedDate >= req.PublishDateFrom.Value || !x.UpdatedDate.HasValue && x.CreatedDate >= req.PublishDateFrom.Value);
			}
			if (req.PublishDateTo.HasValue)
			{
				data = data.Where(x => x.UpdatedDate.HasValue && x.UpdatedDate <= req.PublishDateTo.Value || !x.UpdatedDate.HasValue && x.CreatedDate <= req.PublishDateTo.Value);
			}
			if (req.OrderBy.HasValue)
			{
				switch (req.OrderBy.Value)
				{
					case OrderBy.PublishDate:
						data = (!req.OrderType.HasValue || req.OrderType == OrderType.Ascending) ? data.OrderBy(x => x.CreatedDate) : data.OrderByDescending(x => x.CreatedDate);
						break;
					case OrderBy.Vote:
						data = (!req.OrderType.HasValue || req.OrderType == OrderType.Ascending) ? data.OrderBy(x => x.UpVote - x.DownVote) : data.OrderByDescending(x => x.UpVote - x.DownVote);
						break;
					case OrderBy.View:
						data = (!req.OrderType.HasValue || req.OrderType == OrderType.Ascending) ? data.OrderBy(x => x.ViewCount) : data.OrderByDescending(x => x.ViewCount);
						break;
				}
			}
		}

		private bool ValidateCreateQuestion(string userId, QuestionCreateUpdateModel model, ErrorModel errors, out User user)
		{
			user = _dbContext.Users.Find(userId);
			if (user == null || user.IsDeleted)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "User"));
			}
			else
			{
				if (!model.Tags.Any())
				{
					errors.Add(string.Format(ErrorResource.MissingRequired, "tags"));
				}
				if (model.Title.Length < 10)
				{
					errors.Add(string.Format(ErrorResource.LengthRequired, "title", "10"));
				}
				if (model.Content.Length < 20)
				{
					errors.Add(string.Format(ErrorResource.LengthRequired, "Question content", "20"));
				}
			}

			return errors.IsEmpty;
		}

		private bool ValidateUpdateQuestion(Guid questionId, string userId, QuestionCreateUpdateModel model, ErrorModel errors, out Question question)
		{
			question = _dbContext.Questions.Include(x => x.QuestionTagMaps).FirstOrDefault(x => x.Id == questionId && x.UserId == userId && !x.IsDeleted);
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else
			{
				ValidateCreateQuestion(userId, model, errors, out User user);
			}
			return errors.IsEmpty;
		}

		private bool ValidateVoteQuestion(Guid questionId, string userId, ErrorModel errors, out Question question)
		{
			question = _dbContext.Questions.FirstOrDefault(x => x.Id == questionId && !x.IsDeleted);
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else
			{
				var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
				if (user == null || user.IsDeleted)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
			}
			return errors.IsEmpty;
		}

		private bool ValidateDeleteQuestion(Guid questionId, ErrorModel errors, out Question question, out User user)
		{
			string userId = _userResolverService.GetUser();
			question = _dbContext.Questions.FirstOrDefault(x => x.Id == questionId && !x.IsDeleted && x.UserId == userId);
			user = null;
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else
			{
				user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
				if (user == null || user.IsDeleted)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
			}
			return errors.IsEmpty;
		}

		private bool ValidateDeleteQuestionByAdmin(Guid questionId, string remark, ErrorModel errors, out Question question, out User user)
		{
			string userId = _userResolverService.GetUser();
			question = _dbContext.Questions.Include(x => x.User).FirstOrDefault(x => x.Id == questionId && !x.IsDeleted);
			user = null;
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else if (string.IsNullOrEmpty(remark))
			{
				errors.Add(string.Format(ErrorResource.MissingRequired, "Remark"));
			}
			else
			{
				user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
				if (user == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
			}
			return errors.IsEmpty;
		}

		private bool ValidateRestoreQuestion(Guid questionId, ErrorModel errors, out Question question, out User user)
		{
			string userId = _userResolverService.GetUser();
			question = _dbContext.Questions.Include(x => x.User).FirstOrDefault(x => x.Id == questionId && x.IsDeleted);
			user = null;
			if (question == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Question"));
			}
			else
			{
				user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
				if (user == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
			}
			return errors.IsEmpty;
		}

		private List<string> GetTagNotExist(List<string> tags, out List<Tag> existedTags)
		{
			tags = tags.Select(x => x.ToLower()).ToList();
			existedTags = _dbContext.Tags.Where(x => tags.Contains(x.Name.ToLower())).ToList();
			return tags.Except(existedTags.Select(x => x.Name)).ToList();
		}

		#endregion
	}
}
