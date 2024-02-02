using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Comments;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace Golbaus_BE.Services.Implement
{
	public class CommentService : ICommentService
	{
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;

		public CommentService(ApiDbContext dbContext, UserResolverService userResolverService)
		{
			_dbContext = dbContext;
			_userResolverService = userResolverService;
		}
		#region Post

		public CommentDetailModel CreateCommentPost(CommentCreateModel model, ErrorModel errors)
		{
			if (ValidateCreateCommentPost(model, errors, out Post post, out CommentPost parentComment, out User user))
			{
				CommentPost newComment = model.ParseToPostCommentEntity(post, parentComment, user.Id);
				_dbContext.CommentPosts.Add(newComment);
				_dbContext.SaveChanges();
				CreateNotificationCommentPost(post, newComment, parentComment);
				return new CommentDetailModel(newComment, user);
			}
			return new CommentDetailModel();
		}

		public void UpdateCommentPost(Guid id, CommentUpdateModel model, ErrorModel errors)
		{
			if (ValidateUpdateCommentPost(id, model, errors, out CommentPost comment))
			{
				model.UpdateEntity(comment);
				_dbContext.SaveChanges();
			}
		}

		public PaginationModel<CommentDetailModel> GetPostComments(Guid PostId, PaginationRequest req)
		{
			string userId = _userResolverService.GetUser();
			var comments = _dbContext.CommentPosts.Include(x => x.User).Include(x => x.Childs).Include(x => x.CommentPostUserVoteMaps)
					.Where(x => x.PostId == PostId && !x.IsDeleted && x.ParentId == null).OrderBy(x => x.CreatedDate);
			return new PaginationModel<CommentDetailModel>(req, comments.Select(x => new CommentDetailModel(x, userId, false)));
		}

		public PaginationModel<CommentDetailModel> GetPostCommentReplies(Guid PostId, Guid CommentId, PaginationRequest req)
		{
			string userId = _userResolverService.GetUser();
			var comments = _dbContext.CommentPosts.Include(x => x.User).Include(x => x.Childs).Include(x => x.CommentPostUserVoteMaps)
					.Where(x => x.PostId == PostId && x.ParentId == CommentId && !x.IsDeleted).OrderBy(x => x.CreatedDate);
			return new PaginationModel<CommentDetailModel>(req, comments.Select(x => new CommentDetailModel(x, userId, true)));
		}

		public void DeletePostComment(Guid id, ErrorModel errors)
		{
			if (ValidateDeleteCommentPost(id, errors, out CommentPost comment))
			{
				comment.IsDeleted = true;
				comment.UpdatedDate = DateTimeHelper.GetVietnameTime();

				_dbContext.SaveChanges();
			}
		}

		public void ToggleUpVotePostComment(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVoteComment(id, userId, errors, out CommentPost comment))
			{
				var voted = _dbContext.CommentPostUserVoteMaps.FirstOrDefault(x => x.CommentId == comment.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.CommentPostUserVoteMaps.Add(new CommentPostUserVoteMap { CommentId = comment.Id, UserId = userId, Type = VoteType.UpVote });
					comment.UpVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					voted.Type = VoteType.UpVote;
					comment.UpVote += 1;
					comment.DownVote -= 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					comment.UpVote -= 1;
					_dbContext.CommentPostUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		public void ToggleDownVotePostComment(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVoteComment(id, userId, errors, out CommentPost comment))
			{
				var voted = _dbContext.CommentPostUserVoteMaps.FirstOrDefault(x => x.CommentId == comment.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.CommentPostUserVoteMaps.Add(new CommentPostUserVoteMap { CommentId = comment.Id, UserId = userId, Type = VoteType.DownVote });
					comment.DownVote += 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					voted.Type = VoteType.DownVote;
					comment.UpVote -= 1;
					comment.DownVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					comment.DownVote -= 1;
					_dbContext.CommentPostUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		#region Helper

		private void CreateNotificationCommentPost(Post post, CommentPost comment, CommentPost parentComment)
		{
			List<Notification> notifications = new List<Notification>();
			if (!comment.ParentId.HasValue)
			{
				Notification notification = new Notification()
				{
					CreatedDate = DateTimeHelper.GetVietnameTime(),
					Content = NotificationConstant.NEW_ANSWER,
					IsRead = false,
					IssueId = comment.Id,
					NotifierId = comment.UserId,
					SubscriberId = post.UserId,
					Type = NotificationType.AnswerQuestion,
				};
				if (notification.NotifierId != notification.SubscriberId)
				{
					notifications.Add(notification);
				}
			}
			else
			{
				if (comment.UserId != parentComment.UserId)
				{
					notifications.Add(new Notification()
					{
						CreatedDate = DateTimeHelper.GetVietnameTime(),
						Content = NotificationConstant.REPLY,
						IsRead = false,
						IssueId = comment.Id,
						NotifierId = comment.UserId,
						SubscriberId = parentComment.UserId,
						Type = NotificationType.Reply,
					});
				}

				User userReplyFor = _dbContext.Users.Where(x => x.UserName == comment.ReplyFor).FirstOrDefault();
				if (userReplyFor.Id != parentComment.UserId && comment.UserId != userReplyFor.Id)
				{
					notifications.Add(new Notification()
					{
						CreatedDate = DateTimeHelper.GetVietnameTime(),
						Content = NotificationConstant.REPLY,
						IsRead = false,
						IssueId = comment.Id,
						NotifierId = comment.UserId,
						SubscriberId = userReplyFor.Id,
						Type = NotificationType.Reply,
					});
				}
			}
			_dbContext.Notifications.AddRange(notifications);
			_dbContext.SaveChanges();
		}

		private bool ValidateCreateCommentPost(CommentCreateModel model, ErrorModel errors, out Post post, out CommentPost parentComment, out User user)
		{
			parentComment = null;
			post = null;
			if (ValidateUser(errors, out user))
			{
				post = _dbContext.Posts.FirstOrDefault(x => x.Id == model.PostId);
				if (post == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Post"));
				}
				else if (model.ParentId.HasValue)
				{
					parentComment = _dbContext.CommentPosts.Include(x => x.User).FirstOrDefault(x => x.Id == model.ParentId);
					if (parentComment == null)
					{
						errors.Add(string.Format(ErrorResource.NotFound, "The comment you are trying to reply to"));
					}
				}
			}

			return errors.IsEmpty;
		}

		private bool ValidateUpdateCommentPost(Guid id, CommentUpdateModel model, ErrorModel errors, out CommentPost comment)
		{
			comment = null;
			if (ValidateUser(errors, out User user))
			{
				comment = _dbContext.CommentPosts.FirstOrDefault(x => x.Id == id && x.UserId == user.Id && !x.IsDeleted);
				if (comment == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Comment"));
				}
				else if (string.IsNullOrEmpty(model.Content))
				{
					errors.Add(string.Format(ErrorResource.MissingRequired, "Content"));
				}
			}

			return errors.IsEmpty;
		}


		private bool ValidateDeleteCommentPost(Guid id, ErrorModel errors, out CommentPost comment)
		{
			comment = null;
			if (ValidateUser(errors, out User user))
			{
				comment = _dbContext.CommentPosts.FirstOrDefault(x => x.Id == id && x.UserId == user.Id && !x.IsDeleted);
				if (comment == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Comment"));
				}
			}

			return errors.IsEmpty;
		}

		private bool ValidateVoteComment(Guid id, string userId, ErrorModel errors, out CommentPost comment)
		{
			comment = null;
			var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
			if (user == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "User"));
			}
			else
			{
				comment = _dbContext.CommentPosts.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
				if (comment == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Comment"));
				}
			}

			return errors.IsEmpty;
		}

		private bool ValidateUser(ErrorModel errors, out User user)
		{
			user = _dbContext.Users.FirstOrDefault(x => x.Id == _userResolverService.GetUser());
			if (user == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "User"));
			}
			return errors.IsEmpty;
		}

		#endregion
		#endregion

		#region Question

		public CommentDetailModel CreateCommentQuestion(CommentCreateModel model, ErrorModel errors)
		{
			if (ValidateCreateCommentQuestion(model, errors, out Question question, out CommentQuestion parentComment, out User user))
			{
				CommentQuestion newComment = model.ParseToQuestionCommentEntity(question, parentComment, user.Id);
				_dbContext.CommentQuestions.Add(newComment);
				_dbContext.SaveChanges();
				CreateNotificationAnswerQuestion(question, newComment, parentComment);
				return new CommentDetailModel(newComment, user);
			}
			return new CommentDetailModel();
		}

		public void UpdateCommentQuestion(Guid id, CommentUpdateModel model, ErrorModel errors)
		{
			if (ValidateUpdateCommentQuestion(id, model, errors, out CommentQuestion comment))
			{
				model.UpdateEntity(comment);
				_dbContext.SaveChanges();
			}
		}

		public PaginationModel<CommentDetailModel> GetQuestionComments(Guid QuestionId, PaginationRequest req)
		{
			string userId = _userResolverService.GetUser();
			var comments = _dbContext.CommentQuestions.Include(x => x.User).Include(x => x.Childs).Include(x => x.CommentQuestionUserVoteMaps)
					.Where(x => x.QuestionId == QuestionId && !x.IsDeleted && x.ParentId == null).OrderBy(x => x.CreatedDate);
			return new PaginationModel<CommentDetailModel>(req, comments.Select(x => new CommentDetailModel(x, userId, false)));
		}

		public PaginationModel<CommentDetailModel> GetQuestionCommentReplies(Guid QuestionId, Guid CommentId, PaginationRequest req)
		{
			string userId = _userResolverService.GetUser();
			var comments = _dbContext.CommentQuestions.Include(x => x.User).Include(x => x.Childs).Include(x => x.CommentQuestionUserVoteMaps)
					.Where(x => x.QuestionId == QuestionId && x.ParentId == CommentId && !x.IsDeleted).OrderBy(x => x.CreatedDate);
			return new PaginationModel<CommentDetailModel>(req, comments.Select(x => new CommentDetailModel(x, userId, true)));
		}

		public void DeleteQuestionComment(Guid id, ErrorModel errors)
		{
			if (ValidateDeleteCommentQuestion(id, errors, out CommentQuestion comment))
			{
				comment.IsDeleted = true;
				comment.UpdatedDate = DateTimeHelper.GetVietnameTime();

				_dbContext.SaveChanges();
			}
		}

		public void ToggleUpVoteQuestionComment(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVoteComment(id, userId, errors, out CommentQuestion comment))
			{
				var voted = _dbContext.CommentQuestionUserVoteMaps.FirstOrDefault(x => x.CommentId == comment.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.CommentQuestionUserVoteMaps.Add(new CommentQuestionUserVoteMap { CommentId = comment.Id, UserId = userId, Type = VoteType.UpVote });
					comment.UpVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					voted.Type = VoteType.UpVote;
					comment.UpVote += 1;
					comment.DownVote -= 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					comment.UpVote -= 1;
					_dbContext.CommentQuestionUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		public void ToggleDownVoteQuestionComment(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVoteComment(id, userId, errors, out CommentQuestion comment))
			{
				var voted = _dbContext.CommentQuestionUserVoteMaps.FirstOrDefault(x => x.CommentId == comment.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.CommentQuestionUserVoteMaps.Add(new CommentQuestionUserVoteMap { CommentId = comment.Id, UserId = userId, Type = VoteType.DownVote });
					comment.DownVote += 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					voted.Type = VoteType.DownVote;
					comment.UpVote -= 1;
					comment.DownVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					comment.DownVote -= 1;
					_dbContext.CommentQuestionUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		#region Helper

		private void CreateNotificationAnswerQuestion(Question question, CommentQuestion comment, CommentQuestion parentComment)
		{
			List<Notification> notifications = new List<Notification>();
			if (!comment.ParentId.HasValue)
			{
				Notification notification = new Notification()
				{
					CreatedDate = DateTimeHelper.GetVietnameTime(),
					Content = NotificationConstant.NEW_ANSWER,
					IsRead = false,
					IssueId = comment.Id,
					NotifierId = comment.UserId,
					SubscriberId =  question.UserId,
					Type = NotificationType.AnswerQuestion,
				};
				if (notification.NotifierId != notification.SubscriberId)
				{
					notifications.Add(notification);
				}
			}
			else
			{
				if (comment.UserId != parentComment.UserId)
				{
					notifications.Add(new Notification()
					{
						CreatedDate = DateTimeHelper.GetVietnameTime(),
						Content = NotificationConstant.REPLY,
						IsRead = false,
						IssueId = comment.Id,
						NotifierId = comment.UserId,
						SubscriberId = parentComment.UserId,
						Type = NotificationType.Reply,
					});
				}

				User userReplyFor = _dbContext.Users.Where(x => x.UserName == comment.ReplyFor).FirstOrDefault();
				if (userReplyFor.Id != parentComment.UserId && comment.UserId != userReplyFor.Id)
				{
					notifications.Add(new Notification()
					{
						CreatedDate = DateTimeHelper.GetVietnameTime(),
						Content = NotificationConstant.REPLY,
						IsRead = false,
						IssueId = comment.Id,
						NotifierId = comment.UserId,
						SubscriberId = userReplyFor.Id,
						Type = NotificationType.Reply,
					});
				}
			}
			_dbContext.Notifications.AddRange(notifications);
			_dbContext.SaveChanges();
		}

		private bool ValidateCreateCommentQuestion(CommentCreateModel model, ErrorModel errors, out Question question, out CommentQuestion parentComment, out User user)
		{
			parentComment = null;
			question = null;
			if (ValidateUser(errors, out user))
			{
				question = _dbContext.Questions.FirstOrDefault(x => x.Id == model.PostId);
				if (question == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Question"));
				}
				else if (model.ParentId.HasValue)
				{
					parentComment = _dbContext.CommentQuestions.Include(x => x.User).FirstOrDefault(x => x.Id == model.ParentId);
					if (parentComment == null)
					{
						errors.Add(string.Format(ErrorResource.NotFound, "The comment you are trying to reply to"));
					}
				}
			}

			return errors.IsEmpty;
		}

		private bool ValidateUpdateCommentQuestion(Guid id, CommentUpdateModel model, ErrorModel errors, out CommentQuestion comment)
		{
			comment = null;
			if (ValidateUser(errors, out User user))
			{
				comment = _dbContext.CommentQuestions.FirstOrDefault(x => x.Id == id && x.UserId == user.Id && !x.IsDeleted);
				if (comment == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Comment"));
				}
				else if (string.IsNullOrEmpty(model.Content))
				{
					errors.Add(string.Format(ErrorResource.MissingRequired, "Content"));
				}
			}

			return errors.IsEmpty;
		}


		private bool ValidateDeleteCommentQuestion(Guid id, ErrorModel errors, out CommentQuestion comment)
		{
			comment = null;
			if (ValidateUser(errors, out User user))
			{
				comment = _dbContext.CommentQuestions.FirstOrDefault(x => x.Id == id && x.UserId == user.Id && !x.IsDeleted);
				if (comment == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Comment"));
				}
			}

			return errors.IsEmpty;
		}

		private bool ValidateVoteComment(Guid id, string userId, ErrorModel errors, out CommentQuestion comment)
		{
			comment = null;
			var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
			if (user == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "User"));
			}
			else
			{
				comment = _dbContext.CommentQuestions.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
				if (comment == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "Comment"));
				}
			}

			return errors.IsEmpty;
		}

		#endregion
		#endregion
	}
}
