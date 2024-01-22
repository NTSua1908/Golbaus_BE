using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Comments;

namespace Golbaus_BE.Services.Interface
{
	public interface ICommentService
	{
		//Post
		CommentDetailModel CreateCommentPost(CommentCreateModel model, ErrorModel errors);
		PaginationModel<CommentDetailModel> GetPostComments(Guid PostId, PaginationRequest req);
		PaginationModel<CommentDetailModel> GetPostCommentReplies(Guid PostId, Guid CommentId, PaginationRequest req);
		void UpdateCommentPost(Guid id, CommentUpdateModel model, ErrorModel errors);
		void DeletePostComment(Guid id, ErrorModel errors);
		void ToggleUpVotePostComment(Guid id, ErrorModel errors);
		void ToggleDownVotePostComment(Guid id, ErrorModel errors);

		//Question
		CommentDetailModel CreateCommentQuestion(CommentCreateModel model, ErrorModel errors);
		PaginationModel<CommentDetailModel> GetQuestionComments(Guid QuestionId, PaginationRequest req);
		PaginationModel<CommentDetailModel> GetQuestionCommentReplies(Guid QuestionId, Guid CommentId, PaginationRequest req);
		void UpdateCommentQuestion(Guid id, CommentUpdateModel model, ErrorModel errors);
		void DeleteQuestionComment(Guid id, ErrorModel errors);
		void ToggleUpVoteQuestionComment(Guid id, ErrorModel errors);
		void ToggleDownVoteQuestionComment(Guid id, ErrorModel errors);
	}
}
