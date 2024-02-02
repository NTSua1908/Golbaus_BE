using Golbaus_BE.DTOs;
using Golbaus_BE.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Golbaus_BE.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class NotificationController : BaseController
	{

		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		[HttpGet("GetAllByToken")]
		public IActionResult GetAllByToken([FromQuery] PaginationNotificationRequest req)
		{
			req.Format();
			var posts = _notificationService.GetAllByToken(req);
			return Ok(posts);
		}

		[HttpPut("MarkAllRead")]
		public IActionResult MarkAllRead()
		{
			_notificationService.MarkAllRead();
			return Ok();
		}

		[HttpPut("MarkRead/{id}")]
		public IActionResult MarkRead(Guid id)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_notificationService.MarkRead(id, errors);
			return Ok();
		}

		[HttpPut("MarkUnread/{id}")]
		public IActionResult MarkUnread(Guid id)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_notificationService.MarkUnread(id, errors);
			return Ok();
		}

		[HttpPut("Delete/{id}")]
		public IActionResult Delete(Guid id)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}
			_notificationService.Delete(id, errors);
			return Ok();
		}
	}
}
