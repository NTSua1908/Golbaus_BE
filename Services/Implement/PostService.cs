using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Posts;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Role = Golbaus_BE.Commons.Constants.Role;

namespace Golbaus_BE.Services.Implement
{
	public class PostService : IPostService
	{
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;

		public PostService(ApiDbContext dbContext, UserResolverService userResolverService)
		{
			_dbContext = dbContext;
			_userResolverService = userResolverService;
		}

		public Guid Create(PostCreateModel model, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateCreatePost(userId, model, errors, out User user))
			{
				var newTags = GetTagNotExist(model.Tags, out List<Tag> existedTags);
				Post post = model.ParseToEntity(userId, newTags, existedTags);

				_dbContext.Posts.Add(post);
				user.PostCount++;
				_dbContext.SaveChanges();
				return post.Id;
			}
			return new Guid();
		}

		public void Update(Guid postId, PostCreateModel model, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateUpdatePost(postId, userId, model, errors, out Post post))
			{
				var newTags = GetTagNotExist(model.Tags, out List<Tag> existedTags);
				_dbContext.PostTagMaps.RemoveRange(post.PostTagMaps);
				model.UpdateEntity(post, newTags, existedTags, userId);
				_dbContext.SaveChanges();
			}
		}

		public PostDetailModel GetDetail(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			Post post = _dbContext.Posts.Include(x => x.PostTagMaps).ThenInclude(x => x.Tag)
										.Include(x => x.User).ThenInclude(x => x.UserFollowerMaps)
										.FirstOrDefault(x => x.Id == id && !x.IsDeleted && (x.UserId == userId || x.PublishType == PublishType.Public));
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				return new PostDetailModel(post, userId);
			}
			return new PostDetailModel();
		}

		public void Delete(Guid id, ErrorModel errors)
		{
			if (ValidateDeletePost(id, errors, out Post post, out User user))
			{
				post.IsDeleted = true;
				post.UpdatedBy = user.Id;
				post.UpdatedDate = DateTime.Now;
				_dbContext.SaveChanges();
			}
		}

		public void DeleteByAdmin(Guid id, string remark, ErrorModel errors)
		{
			if (ValidateDeletePostByAdmin(id, remark, errors, out Post post, out User user))
			{
				post.IsDeleted = true;
				post.Remark = remark;
				post.UpdatedBy = user.Id;
				post.UpdatedDate = DateTime.Now;
				_dbContext.SaveChanges();
			}
		}

		public void Restore(Guid id, ErrorModel errors)
		{
			if (ValidateRestorePost(id, errors, out Post post, out User user))
			{
				post.IsDeleted = false;
				post.UpdatedBy = user.Id;
				post.UpdatedDate = DateTime.Now;
				_dbContext.SaveChanges();
			}
		}

		#region Helper

		private bool ValidateCreatePost(string userId, PostCreateModel model, ErrorModel errors, out User user)
		{
			user = _dbContext.Users.Find(userId);
			if (user == null)
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
				if (model.Excerpt.Length < 50)
				{
					errors.Add(string.Format(ErrorResource.LengthRequired, "excerpt", "50"));
				}
				if (model.Content.Length < 50)
				{
					errors.Add(string.Format(ErrorResource.LengthRequired, "post content", "50"));
				}
			}

			return errors.IsEmpty;
		}

		private bool ValidateUpdatePost(Guid postId, string userId, PostCreateModel model, ErrorModel errors, out Post post)
		{
			post = _dbContext.Posts.Include(x => x.PostTagMaps).FirstOrDefault(x => x.Id == postId && x.UserId == userId && !x.IsDeleted);
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				ValidateCreatePost(userId, model, errors, out User user);
			}
			return errors.IsEmpty;
		}

		private bool ValidateDeletePost(Guid postId, ErrorModel errors, out Post post, out User user)
		{
			string userId = _userResolverService.GetUser();
			post = _dbContext.Posts.FirstOrDefault(x => x.Id == postId && !x.IsDeleted && x.UserId == userId);
			user = null;
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				user = _dbContext.Users.Include(x => x.UserRoleMaps).ThenInclude(x => x.Role)
									   .FirstOrDefault(x => x.Id == userId);
				if (user == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
			}
			return errors.IsEmpty;
		}

		private bool ValidateDeletePostByAdmin(Guid postId, string remark, ErrorModel errors, out Post post, out User user)
		{
			string userId = _userResolverService.GetUser();
			post = _dbContext.Posts.FirstOrDefault(x => x.Id == postId && !x.IsDeleted);
			user = null;
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else if (string.IsNullOrEmpty(remark))
			{
				errors.Add(string.Format(ErrorResource.MissingRequired, "Remark"));
			}
			else 
			{
				user = _dbContext.Users.Include(x => x.UserRoleMaps).ThenInclude(x => x.Role)
									   .FirstOrDefault(x => x.Id == userId);
				if (user == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
			}
			return errors.IsEmpty;
		}

		private bool ValidateRestorePost(Guid postId, ErrorModel errors, out Post post, out User user)
		{
			string userId = _userResolverService.GetUser();
			post = _dbContext.Posts.FirstOrDefault(x => x.Id == postId && x.IsDeleted);
			user = null;
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				user = _dbContext.Users.Include(x => x.UserRoleMaps).ThenInclude(x => x.Role)
									   .FirstOrDefault(x => x.Id == userId);
				if (user == null)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
				else if (!user.UserRoleMaps.Any(x =>
				{
					Role role = Enum.Parse<Role>(x.Role.Name);
					return role == Role.Admin || role == Role.SuperAdmin;
				}))
				{
					errors.Add(ErrorResource.DoNotHavePermission);
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
