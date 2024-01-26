using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities;
using System.ComponentModel.DataAnnotations;

namespace Golbaus_BE.DTOs.Posts
{
	public class PostCreateModel
	{
		[Required(ErrorMessage = "Title is required")]
		public string Title { get; set; }
		[Required(ErrorMessage = "Excerpt is required")]
		public string Excerpt { get; set; }
		[Required(ErrorMessage = "Thumbnail is required")]
		public string Thumbnail { get; set; }
		[Required(ErrorMessage = "Content is required")]
		public string Content { get; set; }
		[Required(ErrorMessage = "Tags is required")]
		public List<string> Tags { get; set; }
		[EnumDataType(typeof(PublishType))]
		public PublishType PublishType { get; set; }
		public DateTime? WillBePublishedOn { get; set; }

		public Post ParseToEntity(string userId, List<string> newTags, List<Tag> existedTags)
		{
			List<PostTagMap> postTags = CreatePostTagMaps(newTags, existedTags);

			return new Post
			{
				Title = Title,
				Excerpt = Excerpt,
				Thumbnail = Thumbnail,
				Content = Content,
				PublishType = PublishType,
				CreatedDate = DateTime.Now,
				PublishDate = PublishType == PublishType.Public ? DateTime.Now : WillBePublishedOn.Value.ToLocalTime(),
				PostTagMaps = postTags,
				Remark = "",
				UserId = userId
			};
		}

		public void UpdateEntity(Post post, List<string> newTags, List<Tag> existedTags, string userId)
		{
			post.Title = Title;
			post.Excerpt = Excerpt;
			post.Thumbnail = Thumbnail;
			post.Content = Content;
			post.PublishType = PublishType;
			post.UpdatedDate = DateTime.Now;
			post.PublishDate = !post.PublishDate.HasValue ?
								(PublishType == PublishType.Public ? DateTime.Now : null) :
								post.PublishDate;
			post.PostTagMaps = CreatePostTagMaps(newTags, existedTags);
			post.UpdatedBy = userId;
		}

		private List<PostTagMap> CreatePostTagMaps(List<string> newTags, List<Tag> existedTags)
		{
			List<PostTagMap> postTags = new List<PostTagMap>();
			newTags.ForEach(tag =>
			{
				postTags.Add(new PostTagMap
				{
					Tag = new Tag { Name = tag }
				});
			});
			existedTags.ForEach(tag =>
			{
				postTags.Add(new PostTagMap
				{
					Tag = tag
				});
			});
			return postTags;
		}
	}

	public class PostDetailModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Excerpt { get; set; }
		public string Thumbnail { get; set; }
		public string Content { get; set; }
		public int CountVote { get; set; }
		public DateTime? PublishDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
		public string UserName { get; set; }
		public string Avatar { get; set; }
		public bool IsFollowing { get; set; }
		public bool IsMyPost { get; set; }
		public int PostCount { get; set; }
		public int FollowCount { get; set; }
		public int CommentCount { get; set; }
		public int ViewCount { get; set; }
		public VoteType? Vote { get; set; }
		public PublishType PublishType { get; set; }
		public DateTime? WillBePublishedOn { get; set; }
		public List<string> Tags { get; set; }
        public bool IsMarked { get; set; }

        public PostDetailModel() { }

		public PostDetailModel(Post post, string viewerId)
		{
			Id = post.Id;
			Title = post.Title;
			Excerpt = post.Excerpt;
			Thumbnail = post.Thumbnail;
			Content = post.Content;
			CountVote = post.UpVote - post.DownVote;
			PublishDate = post.PublishDate;
			UpdatedDate = post.UpdatedDate;
			UserId = post.User.Id;
			FullName = post.User.FullName;
			UserName = post.User.UserName;
			Avatar = post.User.Avatar;
			IsMyPost = viewerId == post.UserId;
			PostCount = post.User.Posts.Count(x => x.PublishType == PublishType.Public);
			FollowCount = post.User.UserFollowerMaps.Count();
			CommentCount = post.CommentPosts.Count(x => !x.IsDeleted);
			ViewCount = post.ViewCount;
			var vote = post.PostUserVoteMaps.FirstOrDefault(x => x.PostId == post.Id && x.UserId == viewerId);
			Vote = vote == null ? VoteType.Unvote : vote.Type;
			PublishType = post.PublishType;
			WillBePublishedOn = post.PublishType == PublishType.Schedule ? post.PublishDate : null;
			Tags = post.PostTagMaps.Select(x => x.Tag.Name).ToList();
			IsFollowing = post.User.UserFollowerMaps.Any(x => x.FollowerId == viewerId);
			IsMarked = post.PostBookmarks.Any(x => x.UserId == viewerId && x.PostId == post.Id);
		}
	}

	public class PostListModel
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Excerpt { get; set; }
		public string Thumbnail { get; set; }
		public int UpVote { get; set; }
		public int DownVote { get; set; }
		public int CommentCount { get; set; }
		public int ViewCount { get; set; }
		public DateTime? PublishDate { get; set; }
		public string AuthorName { get; set; }

		public PostListModel(Post post)
		{
			Id = post.Id;
			Title = post.Title;
			Excerpt = post.Excerpt;
			Thumbnail = post.Thumbnail;
			AuthorName = post.User.UserName;
			CommentCount = post.CommentPosts.Count(x => !x.IsDeleted);
			ViewCount = post.ViewCount;
			UpVote = post.UpVote;
			DownVote = post.DownVote;
		}
	}
}
