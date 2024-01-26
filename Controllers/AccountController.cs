using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Users;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Golbaus_BE.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class AccountController : BaseController
	{

		private readonly IAccountService _accountService;
		private readonly UserManager<User> _userManager;
		private readonly UserResolverService _userResolverService;
		private readonly SignInManager<User> _signInManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AccountController(IAccountService accountService, UserResolverService userResolverService,
			UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor)
		{
			_accountService = accountService;
			_userResolverService = userResolverService;
			_userManager = userManager;
			_signInManager = signInManager;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpPost("CreateUser")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}

			errors = await _accountService.CreateUser(model);
			if (!errors.IsEmpty)
				return BadRequest(errors);
			return Ok();
		}

		[HttpPut("UpdateByToken")]
		public IActionResult UpdateByToken([FromBody] UserUpdateByTokenModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}

			_accountService.UpdateByToken(model, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}

		[HttpPut("UpdateAvatarByToken")]
		public IActionResult UpdateAvatarByToken([FromBody] UpdateAvatarModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}

			_accountService.UpdateAvatarByToken(model, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}


		[HttpGet("GetByToken")]
		public IActionResult GetByToken()
		{
			UserGetByTokenModel result = _accountService.GetByToken();
			return Ok(result);
		}

		[HttpGet("GetDetailByToken")]
		public IActionResult GetDetailByToken()
		{
			ErrorModel errors = new ErrorModel();
			GetDetailModel result = _accountService.GetDetailByToken(errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpGet("GetDetailById/{userId}")]
		[AllowAnonymous]
		public IActionResult GetDetailByToken(string userId)
		{
			ErrorModel errors = new ErrorModel();
			GetDetailModel result = _accountService.GetDetailById(userId, errors);
			return errors.IsEmpty ? Ok(result) : BadRequest(errors);
		}

		[HttpPut("ToggleFollow/{userId}")]
		public IActionResult ToggleFollow(string userId)
		{
			ErrorModel errors = new ErrorModel();
			_accountService.ToggleFollow(userId, errors);
			return errors.IsEmpty ? Ok() : BadRequest(errors);
		}
	}
}
