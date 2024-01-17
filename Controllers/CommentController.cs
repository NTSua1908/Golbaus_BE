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
	}
}
