using Golbaus_BE.Commons.Constants;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Posts;
using Golbaus_BE.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Golbaus_BE.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class PostController : BaseController
	{

		private readonly IPostService _postService;

		public PostController(IPostService postService)
		{
			_postService = postService;
		}

		[HttpPost("Create")]
		public IActionResult CreatePost([FromBody] PostCreateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			var result = _postService.Create(model, errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpPut("Update/{postId}")]
		public IActionResult UpdatePost(Guid postId, [FromBody] PostCreateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_postService.Update(postId, model, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetDetail/{postId}")]
		[AllowAnonymous]
		public IActionResult GetDetail(Guid postId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			var result = _postService.GetDetail(postId, errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpDelete("Delete/{postId}")]
		public IActionResult Delete(Guid postId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_postService.Delete(postId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("Restore/{postId}")]
		[Authorize(Roles = RoleContants.Admin + "," + RoleContants.SuperAdmin)]
		public IActionResult Restore(Guid postId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_postService.Restore(postId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpDelete("DeleteByAdmin/{postId}")]
		[Authorize(Roles = RoleContants.Admin + "," + RoleContants.SuperAdmin)]
		public IActionResult DeleteByAdmin(Guid postId, [FromBody] string remark)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_postService.DeleteByAdmin(postId, remark, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("ToggleUpVote/{postId}")]
		public IActionResult ToggleUpVote(Guid postId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_postService.ToggleUpVote(postId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("ToggleDownVote/{postId}")]
		public IActionResult ToggleDownVote(Guid postId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_postService.ToggleDownVote(postId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("IncreaseView/{postId}")]
		[AllowAnonymous]
		public IActionResult IncreaseView(Guid postId)
		{
			ErrorModel errors = new ErrorModel();
			_postService.IncreaseView(postId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetAllByToken")]
		public IActionResult GetAllByToken(PaginationPostQuestionRequest req)
		{
			req.Format();
			var posts = _postService.GetAllByToken(req);
			return Ok(posts);
		}

		[HttpGet("GetAll")]
		[AllowAnonymous]
		public IActionResult GetAll(PaginationPostQuestionRequest req)
		{
			req.Format();
			var posts = _postService.GetAll(req);
			return Ok(posts);
		}

		[HttpGet("GetAllByUser/{userId}")]
		[AllowAnonymous]
		public IActionResult GetAllByUser(string userId, PaginationPostQuestionRequest req)
		{
			req.Format();
			var posts = _postService.GetAllByUser(userId, req);
			return Ok(posts);
		}

		[HttpPut("ToggleAddBookmark/{postId}")]
		public IActionResult ToggleAddBookmark(Guid postId)
		{
			ErrorModel errors = new ErrorModel();
			_postService.ToggleAddBookmark(postId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetAllBookmarkByToken")]
		public IActionResult GetAllBookmarkByToken(PaginationPostQuestionRequest req)
		{
			req.Format();
			var posts = _postService.GetAllBookmarkByToken(req);
			return Ok(posts);
		}

		[HttpGet("GetOtherPostByUser/{userId}/{postId}")]
		[AllowAnonymous]
		public IActionResult GetOtherPostByUser(string userId, Guid postId, PaginationRequest req)
		{
			req.Format();
			var posts = _postService.GetOtherPostByUser(userId, postId, req);
			return Ok(posts);
		}

		[HttpGet("GetRelatedPost/{postId}")]
		[AllowAnonymous]
		public IActionResult GetRelatedPost(Guid postId, List<string> tags, PaginationRequest req)
		{
			req.Format();
			var posts = _postService.GetRelatedPosts(postId, tags, req);
			return Ok(posts);
		}

		[HttpGet("GetNewestPost")]
		[AllowAnonymous]
		public IActionResult GetNewestPost()
		{
			var posts = _postService.GetNewestPosts();
			return Ok(posts);
		}

		[HttpGet("GetPostTrending")]
		[AllowAnonymous]
		public IActionResult GetPostTrending(PaginationRequest req)
		{
			req.Format();
			var posts = _postService.GetPostTrending(req);
			return Ok(posts);
		}

		[HttpGet("GetFeaturedPostByToken")]
		[AllowAnonymous]
		public IActionResult GetFeaturedPostByToken()
		{
			var posts = _postService.GetFeaturedPostByToken();
			return Ok(posts);
		}

		[HttpGet("GetFollowUserPost")]
		public IActionResult GetFollowUserPost(PaginationRequest req)
		{
			req.Format();
			var posts = _postService.GetFollowUserPost(req);
			return Ok(posts);
		}
	}
}
