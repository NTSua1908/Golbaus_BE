using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Comments;
using Golbaus_BE.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Golbaus_BE.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class CommentController : BaseController
	{

		private readonly ICommentService _commentService;

		public CommentController(ICommentService commentService)
		{
			_commentService = commentService;
		}

		[HttpPost("CreateCommentPost")]
		public IActionResult CreateCommentPost([FromBody] CommentCreateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			var result = _commentService.CreateCommentPost(model, errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpPut("Update/Post/{commentId}")]
		public IActionResult UpdateCommentPost(Guid commentId, [FromBody] CommentUpdateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.UpdateCommentPost(commentId, model, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpDelete("Delete/PostComment/{commentId}")]
		public IActionResult DeletePostComment(Guid commentId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.DeletePostComment(commentId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetAll/Post/{postId}")]
		[AllowAnonymous]
		public IActionResult GetAllPostComment(Guid postId, PaginationRequest req)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			req.Format();
			var result = _commentService.GetPostComments(postId, req);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpGet("GetAll/Post/Reply/{postId}/{commentId}")]
		[AllowAnonymous]
		public IActionResult GetPostCommentReplies(Guid postId, Guid commentId, PaginationRequest req)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			req.Format();
			var result = _commentService.GetPostCommentReplies(postId, commentId, req);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpPut("ToggleUpVotePostComment/{commentId}")]
		public IActionResult ToggleUpVotePostComment(Guid commentId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.ToggleUpVotePostComment(commentId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("ToggleDownVotePostComment/{commentId}")]
		public IActionResult ToggleDownVotePostComment(Guid commentId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.ToggleDownVotePostComment(commentId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		//For question

		[HttpPost("CreateCommentQuestion")]
		public IActionResult CreateCommentQuestion([FromBody] CommentCreateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			var result = _commentService.CreateCommentQuestion(model, errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpPut("Update/Question/{commentId}")]
		public IActionResult UpdateCommentQuestion(Guid commentId, [FromBody] CommentUpdateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.UpdateCommentQuestion(commentId, model, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpDelete("Delete/QuestionComment/{commentId}")]
		public IActionResult DeleteQuestionComment(Guid commentId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.DeleteQuestionComment(commentId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetAll/Question/{questionId}")]
		[AllowAnonymous]
		public IActionResult GetAllQuestionComment(Guid questionId, PaginationRequest req)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			req.Format();
			var result = _commentService.GetQuestionComments(questionId, req);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpGet("GetAll/Question/Reply/{questionId}/{commentId}")]
		[AllowAnonymous]
		public IActionResult GetQuestionCommentReplies(Guid questionId, Guid commentId, PaginationRequest req)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			req.Format();
			var result = _commentService.GetQuestionCommentReplies(questionId, commentId, req);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpPut("ToggleUpVoteQuestionComment/{commentId}")]
		public IActionResult ToggleUpVoteQuestionComment(Guid commentId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.ToggleUpVoteQuestionComment(commentId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("ToggleDownVoteQuestionComment/{commentId}")]
		public IActionResult ToggleDownVoteQuestionComment(Guid commentId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_commentService.ToggleDownVoteQuestionComment(commentId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}
	}
}
