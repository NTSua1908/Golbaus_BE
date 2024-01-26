using Golbaus_BE.DTOs.Questions;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Posts;

namespace Golbaus_BE.Services.Interface
{
	public interface IQuestionService
	{
		Guid Create(QuestionCreateUpdateModel model, ErrorModel errors);
		void Update(Guid postId, QuestionCreateUpdateModel model, ErrorModel errors);
		QuestionDetailModel GetDetail(Guid id, ErrorModel errors);
		void Delete(Guid id, ErrorModel errors);
		void DeleteByAdmin(Guid id, string remark, ErrorModel errors);
		void Restore(Guid id, ErrorModel errors);
		void ToggleUpVote(Guid id, ErrorModel errors);
		void ToggleDownVote(Guid id, ErrorModel errors);
		void IncreaseView(Guid id, ErrorModel errors);
		PaginationModel<QuestionListModel> GetAll(PaginationPostQuestionRequest req);
		PaginationModel<QuestionListModel> GetAllByToken(PaginationPostQuestionRequest req);
		PaginationModel<QuestionListModel> GetAllByUser(string userId, PaginationPostQuestionRequest req);
		void ToggleAddBookmark(Guid id, ErrorModel errors);
	}
}
