using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Comments;

namespace Golbaus_BE.Services.Interface
{
	public interface ICommentService
	{
		CommentDetailModel CreateCommentPost(CommentCreateModel model, ErrorModel errors);
		PaginationModel<CommentDetailModel> GetPostComments(Guid PostId, PaginationRequest req);
		PaginationModel<CommentDetailModel> GetPostCommentReplies(Guid PostId, Guid CommentId, PaginationRequest req);
		void UpdateCommentPost(Guid id, CommentUpdateModel model, ErrorModel errors);
		void DeletePostComment(Guid id, ErrorModel errors);
	}
}
