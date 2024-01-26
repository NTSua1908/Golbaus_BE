using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Posts;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Role = Golbaus_BE.Commons.Constants.Role;

namespace Golbaus_BE.Services.Implement
{
	public class PostService : IPostService
	{
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PostService(ApiDbContext dbContext, UserResolverService userResolverService, IHttpContextAccessor httpContextAccessor)
		{
			_dbContext = dbContext;
			_userResolverService = userResolverService;
			_httpContextAccessor = httpContextAccessor;
		}

		public Guid Create(PostCreateModel model, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateCreatePost(userId, model, errors, out User user))
			{
				var newTags = GetTagNotExist(model.Tags, out List<Tag> existedTags);
				Post post = model.ParseToEntity(userId, newTags, existedTags);

				_dbContext.Posts.Add(post);
				if (model.PublishType == PublishType.Schedule)
				{
					BackgroundJob.Schedule<IPostService>(x => x.PublishTask(post.Id), (post.PublishDate - DateTime.Now).Value);
				}

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
										.Include(x => x.User).ThenInclude(x => x.Posts)
										.Include(x => x.PostUserVoteMaps)
										.Include(x => x.CommentPosts)
										.Include(x => x.PostBookmarks)
										.FirstOrDefault(x => x.Id == id && !x.IsDeleted && (x.UserId == userId || x.PublishType == PublishType.Public));
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				_httpContextAccessor.HttpContext.Session.SetString("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				_httpContextAccessor.HttpContext.Session.SetString("PostId", id.ToString());
				Console.WriteLine(_httpContextAccessor.HttpContext.Session.Id);
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

		public void ToggleUpVote(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVotePost(id, userId, errors, out Post post))
			{
				var voted = _dbContext.PostUserVoteMaps.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.PostUserVoteMaps.Add(new PostUserVoteMap { PostId = post.Id, UserId = userId, Type = VoteType.UpVote });
					post.UpVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					voted.Type = VoteType.UpVote;
					post.UpVote += 1;
					post.DownVote -= 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					post.UpVote -= 1;
					_dbContext.PostUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		public void ToggleDownVote(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			if (ValidateVotePost(id, userId, errors, out Post post))
			{
				var voted = _dbContext.PostUserVoteMaps.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
				if (voted == null)
				{
					_dbContext.PostUserVoteMaps.Add(new PostUserVoteMap { PostId = post.Id, UserId = userId, Type = VoteType.DownVote });
					post.DownVote += 1;
				}
				else if (voted.Type == VoteType.UpVote)
				{
					voted.Type = VoteType.DownVote;
					post.UpVote -= 1;
					post.DownVote += 1;
				}
				else if (voted.Type == VoteType.DownVote)
				{
					post.DownVote -= 1;
					_dbContext.PostUserVoteMaps.Remove(voted);
				}
				_dbContext.SaveChanges();
			}
		}

		public void PublishTask(Guid id)
		{
			Post post = _dbContext.Posts.FirstOrDefault(x => x.Id == id);
			if (post != null)
			{
				post.PublishDate = DateTime.Now;
				post.PublishType = PublishType.Public;
				_dbContext.SaveChanges();
			}
		}

		public void IncreaseView(Guid id, ErrorModel errors)
		{
			Post post = _dbContext.Posts.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				string timeGet = _httpContextAccessor.HttpContext.Session.GetString("Time");
				string postId = _httpContextAccessor.HttpContext.Session.GetString("PostId");
				Console.WriteLine(_httpContextAccessor.HttpContext.Session.Id);
				if (timeGet != null && postId != null &&
					Guid.TryParse(postId, out Guid PostId) && DateTime.TryParse(timeGet, out DateTime TimeGet) &&
					id == PostId && (DateTime.Now - TimeGet).TotalSeconds > 10)
				{
					post.ViewCount++;
					_dbContext.SaveChanges();
					_httpContextAccessor.HttpContext.Session.Remove("Time");
					_httpContextAccessor.HttpContext.Session.Remove("PostId");
				}
				else
				{
					errors.Add(string.Format(ErrorResource.Invalid, "Post"));
				}
			}
		}

		public PaginationModel<PostListModel> GetAll(PaginationPostQuestionRequest req)
		{
			var posts = _dbContext.Posts.Include(x => x.CommentPosts).Include(x => x.User)
										.Include(x => x.PostTagMaps).ThenInclude(x => x.Tag)
										.Where(x => !x.IsDeleted && x.PublishType == PublishType.Public);
			Filter(req, ref posts);

			if (req.Tags != null)
			{
				var result = posts.AsEnumerable();
				req.Tags = req.Tags.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToList();
				result = result.Where(p => req.Tags.All(t => p.PostTagMaps.Any(m => m.Tag.Name.ToLower().Contains(t))));
				return new PaginationModel<PostListModel>(req, result.Select(x => new PostListModel(x)));
			}

			return new PaginationModel<PostListModel>(req, posts.Select(x => new PostListModel(x)));
		}

		public PaginationModel<PostListModel> GetAllByToken(PaginationPostQuestionRequest req)
		{
			string userId = _userResolverService.GetUser();
			var posts = _dbContext.Posts.Include(x => x.CommentPosts).Include(x => x.User)
										.Include(x => x.PostTagMaps).ThenInclude(x => x.Tag)
										.Where(x => x.UserId == userId && !x.IsDeleted);
			Filter(req, ref posts);

			if (req.Tags != null)
			{
				var result = posts.AsEnumerable();
				req.Tags = req.Tags.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToList();
				result = result.Where(p => req.Tags.All(t => p.PostTagMaps.Any(m => m.Tag.Name.ToLower().Contains(t))));
				return new PaginationModel<PostListModel>(req, result.Select(x => new PostListModel(x)));
			}

			return new PaginationModel<PostListModel>(req, posts.Select(x => new PostListModel(x)));
		}

		public PaginationModel<PostListModel> GetAllByUser(string userId, PaginationPostQuestionRequest req)
		{
			var posts = _dbContext.Posts.Include(x => x.CommentPosts).Include(x => x.User)
										.Include(x => x.PostTagMaps).ThenInclude(x => x.Tag)
										.Where(x => x.UserId == userId && !x.IsDeleted && x.PublishType == PublishType.Public);
			Filter(req, ref posts);

			if (req.Tags != null)
			{
				var result = posts.AsEnumerable();
				req.Tags = req.Tags.Where(x => !string.IsNullOrEmpty(x)).Select(x => x.ToLower()).ToList();
				result = result.Where(p => req.Tags.All(t => p.PostTagMaps.Any(m => m.Tag.Name.ToLower().Contains(t))));
				return new PaginationModel<PostListModel>(req, result.Select(x => new PostListModel(x)));
			}

			return new PaginationModel<PostListModel>(req, posts.Select(x => new PostListModel(x)));
		}

		public void ToggleAddBookmark(Guid id, ErrorModel errors)
		{
			Post post = _dbContext.Posts.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				string userId = _userResolverService.GetUser();
				var mark = _dbContext.PostBookmarks.FirstOrDefault(x => x.UserId == userId && x.PostId == id);
				if (mark == null)
				{
					_dbContext.PostBookmarks.Add(new PostBookmark()
					{
						UserId = userId,
						PostId = id,
					});
				}
				else
				{
					_dbContext.PostBookmarks.Remove(mark);
				}
				_dbContext.SaveChanges();
			}
		}

		#region Helper

		private void Filter(PaginationPostQuestionRequest req, ref IQueryable<Post> data)
		{
			if (!string.IsNullOrEmpty(req.SearchText))
			{
				req.SearchText = req.SearchText.ToLower();
				data = data.Where(x => x.Title.ToLower().Contains(req.SearchText));
			}
			if (req.PublishDateFrom.HasValue)
			{
				data = data.Where(x => x.PublishDate.HasValue && x.PublishDate.Value >= req.PublishDateFrom.Value);
			}
			if (req.PublishDateTo.HasValue)
			{
				data = data.Where(x => x.PublishDate.HasValue && x.PublishDate.Value <= req.PublishDateTo.Value);
			}
			if (req.OrderBy.HasValue)
			{
				switch (req.OrderBy.Value)
				{
					case OrderBy.PublishDate:
						data = (!req.OrderType.HasValue || req.OrderType == OrderType.Ascending) ? data.OrderBy(x => x.PublishDate) : data.OrderByDescending(x => x.PublishDate);
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

		private bool ValidateCreatePost(string userId, PostCreateModel model, ErrorModel errors, out User user)
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
				if (model.Excerpt.Length < 50)
				{
					errors.Add(string.Format(ErrorResource.LengthRequired, "excerpt", "50"));
				}
				if (model.Content.Length < 50)
				{
					errors.Add(string.Format(ErrorResource.LengthRequired, "post content", "50"));
				}
				if (model.PublishType == PublishType.Schedule && (model.WillBePublishedOn - DateTime.UtcNow).Value.TotalMinutes < 30)
				{
					errors.Add(ErrorResource.PublishTime);
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
				if (model.PublishType == PublishType.Schedule)
				{
					errors.Add(string.Format(ErrorResource.Invalid, "Publish type"));
				}
				ValidateCreatePost(userId, model, errors, out User user);
			}
			return errors.IsEmpty;
		}

		private bool ValidateVotePost(Guid postId, string userId, ErrorModel errors, out Post post)
		{
			post = _dbContext.Posts.FirstOrDefault(x => x.Id == postId && !x.IsDeleted);
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
			}
			else
			{
				var user = _dbContext.Users.Include(x => x.UserRoleMaps).ThenInclude(x => x.Role)
									   .FirstOrDefault(x => x.Id == userId);
				if (user == null || user.IsDeleted)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
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
				user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
				if (user == null || user.IsDeleted)
				{
					errors.Add(string.Format(ErrorResource.NotFound, "User"));
				}
			}
			return errors.IsEmpty;
		}

		private bool ValidateDeletePostByAdmin(Guid postId, string remark, ErrorModel errors, out Post post, out User user)
		{
			string userId = _userResolverService.GetUser();
			post = _dbContext.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == postId && !x.IsDeleted);
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
				user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
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
			post = _dbContext.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == postId && x.IsDeleted);
			user = null;
			if (post == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Post"));
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
