using Golbaus_BE.Commons.Constants;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Posts;
using Golbaus_BE.DTOs.Questions;

namespace Golbaus_BE.Services.Interface
{
	public interface IPostService
	{
		Guid Create(PostCreateModel model, ErrorModel errors);
		void Update(Guid postId, PostCreateModel model, ErrorModel errors);
		PostDetailModel GetDetail(Guid id, ErrorModel errors);
		void Delete(Guid id, ErrorModel errors);
		void DeleteByAdmin(Guid id, string remark, ErrorModel errors);
		void Restore(Guid id, ErrorModel errors);
		void ToggleUpVote(Guid id, ErrorModel errors);
		void ToggleDownVote(Guid id, ErrorModel errors);
		void PublishTask(Guid id);
		void IncreaseView(Guid id, ErrorModel errors);
		PaginationModel<PostListModel> GetAll(PaginationPostQuestionRequest req);
		PaginationModel<PostListModel> GetAllByToken(PaginationPostQuestionRequest req);
		PaginationModel<PostListModel> GetAllByUser(string userId, PaginationPostQuestionRequest req);
		void ToggleAddBookmark(Guid id, ErrorModel errors);
		PaginationModel<PostListModel> GetAllBookmarkByToken(PaginationPostQuestionRequest req);
		PaginationModel<PostListModel> GetOtherPostByUser(string userId, Guid postId, PaginationRequest req);
		PaginationModel<PostListModel> GetRelatedPosts(Guid postId, List<string> tags, PaginationRequest req);
		List<PostListModel> GetNewestPosts();
		List<List<PostBlockModel>> GetFeaturedPostByToken();
		PaginationModel<PostBlockModel> GetPostTrending(PaginationRequest req);
		PaginationModel<PostListModel> GetFollowUserPost(PaginationRequest req);
	}
}
