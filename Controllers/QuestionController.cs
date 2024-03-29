﻿using Golbaus_BE.Commons.Constants;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Questions;
using Golbaus_BE.Services.Implement;
using Golbaus_BE.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Golbaus_BE.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class QuestionController : BaseController
	{

		private readonly IQuestionService _questionService;

		public QuestionController(IQuestionService questionService)
		{
			_questionService = questionService;
		}

		[HttpPost("Create")]
		public IActionResult Create([FromBody] QuestionCreateUpdateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			var result = _questionService.Create(model, errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpPut("Update/{questionId}")]
		public IActionResult Update(Guid questionId, [FromBody] QuestionCreateUpdateModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_questionService.Update(questionId, model, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetDetail/{questionId}")]
		[AllowAnonymous]
		public IActionResult GetDetail(Guid questionId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			var result = _questionService.GetDetail(questionId, errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpDelete("Delete/{questionId}")]
		public IActionResult Delete(Guid questionId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_questionService.Delete(questionId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("Restore/{questionId}")]
		[Authorize(Roles = RoleContants.Admin + "," + RoleContants.SuperAdmin)]
		public IActionResult Restore(Guid questionId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_questionService.Restore(questionId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpDelete("DeleteByAdmin/{questionId}")]
		[Authorize(Roles = RoleContants.Admin + "," + RoleContants.SuperAdmin)]
		public IActionResult DeleteByAdmin(Guid questionId, [FromBody] string remark)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_questionService.DeleteByAdmin(questionId, remark, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("ToggleUpVote/{questionId}")]
		public IActionResult ToggleUpVote(Guid questionId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_questionService.ToggleUpVote(questionId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("ToggleDownVote/{questionId}")]
		public IActionResult ToggleDownVote(Guid questionId)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_questionService.ToggleDownVote(questionId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("IncreaseView/{questionId}")]
		[AllowAnonymous]
		public IActionResult IncreaseView(Guid questionId)
		{
			ErrorModel errors = new ErrorModel();
			_questionService.IncreaseView(questionId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetAllByToken")]
		public IActionResult GetAllByToken(PaginationPostQuestionRequest req)
		{
			req.Format();
			var questions = _questionService.GetAllByToken(req);
			return Ok(questions);
		}

		[HttpGet("GetAll")]
		[AllowAnonymous]
		public IActionResult GetAll(PaginationPostQuestionRequest req)
		{
			req.Format();
			var questions = _questionService.GetAll(req);
			return Ok(questions);
		}

		[HttpGet("GetAllByUser/{userId}")]
		[AllowAnonymous]
		public IActionResult GetAllByUser(string userId, PaginationPostQuestionRequest req)
		{
			req.Format();
			var questions = _questionService.GetAllByUser(userId, req);
			return Ok(questions);
		}

		[HttpPut("ToggleAddBookmark/{questionId}")]
		public IActionResult ToggleAddBookmark(Guid questionId)
		{
			ErrorModel errors = new ErrorModel();
			_questionService.ToggleAddBookmark(questionId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpGet("GetAllBookmarkByToken")]
		public IActionResult GetAllBookmarkByToken(PaginationPostQuestionRequest req)
		{
			req.Format();
			var questions = _questionService.GetAllBookmarkByToken(req);
			return Ok(questions);
		}

		[HttpGet("GetRelatedQuestion/{questionId}")]
		[AllowAnonymous]
		public IActionResult GetRelatedQuestions(Guid questionId, List<string> tags, PaginationRequest req)
		{
			req.Format();
			var questions = _questionService.GetRelatedQuestions(questionId, tags, req);
			return Ok(questions);
		}

		[HttpGet("GetNewestQuestion")]
		[AllowAnonymous]
		public IActionResult GetNewestQuestions(PaginationRequest req)
		{
			req.Format();
			var questions = _questionService.GetNewestQuestions(req);
			return Ok(questions);
		}

		[HttpGet("GetFeaturedQuestionByToken")]
		[AllowAnonymous]
		public IActionResult GetFeaturedQuestionByToken(PaginationRequest req)
		{
			req.Format();
			var questions = _questionService.GetFeaturedQuestionByToken(req);
			return Ok(questions);
		}

		[HttpGet("GetFollowUserQuestion")]
		public IActionResult GetFollowUserQuestion(PaginationRequest req)
		{
			req.Format();
			var questions = _questionService.GetFollowUserQuestion(req);
			return Ok(questions);
		}
	}
}
