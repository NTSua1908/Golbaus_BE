using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Comments;
using Golbaus_BE.Entities;
using Golbaus_BE.Entities.BaseEntity;
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

		public CommentDetailModel CreateCommentPost(CommentCreateModel model, ErrorModel errors)
		{
			if (ValidateCreateCommentPost(model, errors, out Post post, out CommentPost comment, out User user))
			{
				CommentPost newComment = model.ParseToPostCommentEntity(post, comment, user.Id);
				_dbContext.CommentPosts.Add(newComment);
				_dbContext.SaveChanges();
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
				comment.UpdatedDate = DateTime.Now;

				_dbContext.SaveChanges();
			}
		}

		#region Helper
		private bool ValidateCreateCommentPost(CommentCreateModel model, ErrorModel errors, out Post post, out CommentPost comment, out User user)
		{
			comment = null;
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
					comment = _dbContext.CommentPosts.Include(x => x.User).FirstOrDefault(x => x.Id == model.ParentId);
					if (comment == null)
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
	}
}
