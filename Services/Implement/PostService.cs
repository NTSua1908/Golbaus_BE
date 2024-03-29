﻿using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Posts;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Hangfire;
using Microsoft.EntityFrameworkCore;

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
				Post post = model.ParseToEntity(user, newTags, existedTags);

				_dbContext.Posts.Add(post);
				_dbContext.SaveChanges();

				if (model.PublishType == PublishType.Schedule)
				{
					BackgroundJob.Schedule<IPostService>(x => x.PublishTask(post.Id), (post.PublishDate - DateTimeHelper.GetVietnameTime()).Value);
				}
				else if (model.PublishType == PublishType.Public)
				{
					CreateNotificationNewPost(post);
					AddNewestPost(post);
				}

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
				_httpContextAccessor.HttpContext.Session.SetString("Time", DateTimeHelper.GetVietnameTime().ToString("yyyy-MM-dd HH:mm:ss"));
				_httpContextAccessor.HttpContext.Session.SetString("PostId", id.ToString());

				var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
				if (user != null)
				{
					var recentlyViewedTags = user.RecentlyViewedTags?.Split("|+|").ToList() ?? new List<string>();

					//store tag recent viewed can >= 100
					if (recentlyViewedTags.Count >= 100)
					{
						recentlyViewedTags.RemoveRange(0, post.PostTagMaps.Count);
						recentlyViewedTags.AddRange(post.PostTagMaps.Select(x => x.Tag.Name));
					}
					else
					{
						recentlyViewedTags.AddRange(post.PostTagMaps.Select(x => x.Tag.Name));
					}
					user.RecentlyViewedTags = string.Join("|+|", recentlyViewedTags);
					_dbContext.SaveChanges();
				}

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
				post.UpdatedDate = DateTimeHelper.GetVietnameTime();
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
				post.UpdatedDate = DateTimeHelper.GetVietnameTime();
				_dbContext.SaveChanges();
			}
		}

		public void Restore(Guid id, ErrorModel errors)
		{
			if (ValidateRestorePost(id, errors, out Post post, out User user))
			{
				post.IsDeleted = false;
				post.UpdatedBy = user.Id;
				post.UpdatedDate = DateTimeHelper.GetVietnameTime();
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
			Post post = _dbContext.Posts.Include(x => x.User).ThenInclude(x => x.UserFollowerMaps).FirstOrDefault(x => x.Id == id);
			if (post != null)
			{
				post.PublishDate = DateTimeHelper.GetVietnameTime();
				post.PublishType = PublishType.Public;

				CreateNotificationNewPost(post);
				AddNewestPost(post);

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
			else if (post.PublishType == PublishType.Public)
			{
				string timeGet = _httpContextAccessor.HttpContext.Session.GetString("Time");
				string postId = _httpContextAccessor.HttpContext.Session.GetString("PostId");
				Console.WriteLine(_httpContextAccessor.HttpContext.Session.Id);
				if (timeGet != null && postId != null &&
					Guid.TryParse(postId, out Guid PostId) && DateTime.TryParse(timeGet, out DateTime TimeGet) &&
					id == PostId && (DateTimeHelper.GetVietnameTime() - TimeGet).TotalSeconds > 10)
				{
					post.ViewCount++;
					AddViewTrending(post);

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
			Post post = _dbContext.Posts.FirstOrDefault(x => x.Id == id && !x.IsDeleted && x.PublishType == PublishType.Public);
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
						MarkedDate = DateTimeHelper.GetVietnameTime()
					});
				}
				else
				{
					_dbContext.PostBookmarks.Remove(mark);
				}
				_dbContext.SaveChanges();
			}
		}

		public PaginationModel<PostListModel> GetAllBookmarkByToken(PaginationPostQuestionRequest req)
		{
			string userId = _userResolverService.GetUser();
			var posts = _dbContext.PostBookmarks
				.Include(x => x.Post).ThenInclude(x => x.CommentPosts)
				.Include(x => x.Post).ThenInclude(x => x.PostTagMaps).ThenInclude(x => x.Tag)
				.Include(x => x.Post).ThenInclude(x => x.User)
				.Where(x => x.UserId == userId && !x.Post.IsDeleted && x.Post.PublishType == PublishType.Public)
				.OrderByDescending(x => x.MarkedDate)
				.Select(x => x.Post);

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

		public PaginationModel<PostListModel> GetOtherPostByUser(string userId, Guid postId, PaginationRequest req)
		{
			var posts = _dbContext.Posts.Include(x => x.CommentPosts).Include(x => x.User)
										.Include(x => x.PostTagMaps).ThenInclude(x => x.Tag)
										.Where(x => x.UserId == userId && !x.IsDeleted && x.PublishType == PublishType.Public && x.Id != postId)
										.OrderByDescending(x => x.ViewCount);
			return new PaginationModel<PostListModel>(req, posts.Select(x => new PostListModel(x)));
		}

		public PaginationModel<PostListModel> GetRelatedPosts(Guid postId, List<string> tags, PaginationRequest req)
		{
			tags = tags.Select(x => x.ToLower()).ToList();
			var posts = _dbContext.Posts.Include(x => x.CommentPosts).Include(x => x.User)
										.Include(x => x.PostTagMaps).ThenInclude(x => x.Tag)
										.Where(x => !x.IsDeleted && x.PublishType == PublishType.Public && x.Id != postId && x.PostTagMaps.Any(x => tags.Contains(x.Tag.Name.ToLower())))
										.OrderByDescending(x => x.ViewCount);
			return new PaginationModel<PostListModel>(req, posts.Select(x => new PostListModel(x)));
		}

		public List<PostListModel> GetNewestPosts()
		{
			return _dbContext.NewestPosts.Where(x => !x.Post.IsDeleted)
				.OrderByDescending(x => x.PublishDate)
				.Include(x => x.Post).ThenInclude(x => x.User)
				.Include(x => x.Post).ThenInclude(x => x.CommentPosts)
				.Take(15).Select(x => new PostListModel(x.Post)).ToList();
		}

		public PaginationModel<PostBlockModel> GetPostTrending(PaginationRequest req)
		{
			var trendings = _dbContext.TrendingPosts.Include(x => x.Post).ThenInclude(x => x.User)
				.Where(x => !x.Post.IsDeleted)
				.GroupBy(x => x.PostId)
				.Select(x => new {
					Id = x.Key,
					x.First().Post.Title,
					Author = x.First().Post.User.UserName,
					Date = x.First().Post.PublishDate,
					ViewCount = x.Sum(x => x.ViewCount),
					x.First().Post.Thumbnail,
					x.First().Post.Excerpt
				})
				.OrderByDescending(x => x.ViewCount).ThenByDescending(x => x.Date)
				.Select(x => new PostBlockModel
				{
					Id = x.Id,
					Title = x.Title,
					Date = x.Date.Value,
					Author = x.Author,
					Thumbnail = x.Thumbnail,
					Excerpt = x.Excerpt
				}).ToList();

			return new PaginationModel<PostBlockModel>(req, trendings);
		}

		public List<List<PostBlockModel>> GetFeaturedPostByToken()
		{
			string userId = _userResolverService.GetUser();
			var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
			if (user != null && !string.IsNullOrEmpty(user.RecentlyViewedTags))
			{
				var recentViewdTags = user.RecentlyViewedTags.Split("|+|").Select(x => x.ToLower()).ToList();
				var newestPosts = _dbContext.NewestPosts.Where(x => !x.Post.IsDeleted)
				.Select(x => new
				{
					Id = x.PostId,
					Title = x.Post.Title,
					Excerpt = x.Post.Excerpt,
					Thumbnail = x.Post.Thumbnail,
					Author = x.Post.User.UserName,
					Date = x.Post.PublishDate,
					Tags = x.Post.PostTagMaps.Select(x => x.Tag.Name.ToLower())
				}).ToList();
				var featuredPosts = newestPosts.Where(x => recentViewdTags.Any(tag => x.Tags.Contains(tag)))
					.Select(x => new PostBlockModel(x.Id, x.Title, x.Excerpt, x.Thumbnail, x.Author, x.Date.Value)).ToList();

				if (featuredPosts.Count < 20)
				{
					featuredPosts.AddRange(GetPostTrending(new PaginationRequest { Page = 0, Amount = (32 + 4 - (featuredPosts.Count % 4)) }).Data);
					featuredPosts = featuredPosts.DistinctBy(x => x.Id).ToList();
				}

				return featuredPosts.OrderByDescending(x => x.Date).Select((post, index) => new { post, index })
					.GroupBy(x => x.index / 4)
					.Select(group => group.Select(x => x.post).ToList())
					.ToList();
			}
			else
			{
				var newestPosts = _dbContext.NewestPosts.OrderByDescending(x => x.Post.ViewCount).Select(x => new
				{
					Id = x.PostId,
					Title = x.Post.Title,
					Excerpt = x.Post.Excerpt,
					Thumbnail = x.Post.Thumbnail,
					Author = x.Post.User.UserName,
					Date = x.Post.PublishDate,
					Tags = x.Post.PostTagMaps.Select(x => x.Tag.Name)
				}).ToList();
				return newestPosts
					.Select(x => new PostBlockModel(x.Id, x.Title, x.Excerpt, x.Thumbnail, x.Author, x.Date.Value))
					.Select((post, index) => new { post, index })
					.GroupBy(x => x.index / 4)
					.Select(group => group.Select(x => x.post).ToList())
					.ToList();
			}
		}

		public PaginationModel<PostListModel> GetFollowUserPost(PaginationRequest req)
		{
			string userId = _userResolverService.GetUser();
			var user = _dbContext.Users.Include(x => x.UserFollowingMaps).ThenInclude(x => x.Follower)
									.ThenInclude(x => x.Posts).ThenInclude(x => x.CommentPosts)
									.FirstOrDefault(x => x.Id == userId);
			if (user != null)
			{
				var posts = user.UserFollowingMaps.SelectMany(x => x.Follower.Posts.Where(x => x.PublishType == PublishType.Public && !x.IsDeleted))
								.OrderByDescending(x => x.PublishDate);
				return new PaginationModel<PostListModel>(req, posts.Select(x => new PostListModel(x)));
			}

			return new PaginationModel<PostListModel>();
		}

		#region Helper

		private void CreateNotificationNewPost(Post post)
		{
			List<Notification> notifications = new List<Notification>();
			foreach (var userFollow in post.User.UserFollowerMaps)
			{
				Notification notification = new Notification()
				{
					CreatedDate = DateTimeHelper.GetVietnameTime(),
					Content = NotificationConstant.NEW_POST,
					IsRead = false,
					IssueId = post.Id,
					NotifierId = post.User.Id,
					SubscriberId = userFollow.FollowerId,
					Type = NotificationType.NewPost,
				};
				notifications.Add(notification);
			}
			_dbContext.Notifications.AddRange(notifications);
			_dbContext.SaveChanges();
		}

		private void AddNewestPost(Post post)
		{
			var posts = _dbContext.NewestPosts.OrderByDescending(x => x.PublishDate).ToList();
			
			if (posts.Count >= 100)
			{ 	
				_dbContext.NewestPosts.Remove(posts.Last());
			}

			_dbContext.NewestPosts.Add(new NewestPost
			{
				Post = post,
				PublishDate = post.PublishDate.Value
			});

			_dbContext.SaveChanges();
		}

		private void AddViewTrending(Post post)
		{
			var now = DateTimeHelper.GetVietnameTime();
			var trendingPost = _dbContext.TrendingPosts.FirstOrDefault(x => x.Date == now.Date && x.PostId == post.Id);

			if (trendingPost != null)
			{
				trendingPost.ViewCount += 1;
			}
			else
			{
				_dbContext.TrendingPosts.Add(new TrendingPost 
				{
					Date = now.Date,
					Post = post,
					ViewCount = 1
				});
			}
		}

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
			else
			{
				data = data.OrderByDescending(x => x.PublishDate);
			}
		}

		private bool ValidateCreatePost(string userId, PostCreateModel model, ErrorModel errors, out User user)
		{
			user = _dbContext.Users.Where(x => x.Id == userId)
									.Include(x => x.UserFollowerMaps)
									.FirstOrDefault();

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
				if (model.PublishType == PublishType.Schedule && (!model.WillBePublishedOn.HasValue || (DateTimeHelper.ConvertVietnameTime(model.WillBePublishedOn.Value) - DateTimeHelper.GetVietnameTime()).TotalMinutes < 30))
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
