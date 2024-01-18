using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Golbaus_BE.DTOs.Auths;
using Golbaus_BE.Commons.Constants;
using Role = Golbaus_BE.Commons.Constants.Role;
using Golbaus_BE.Services.Implement;
using Golbaus_BE.DTOs.Users;
using Hangfire;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace Golbaus_BE.Controllers
{
	[Produces("application/json")]
	[Route("api/[controller]")]
	public class AuthController : BaseController
	{

		private readonly IAuthServices _authServices;
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IAccountService _accountService;
		private readonly IEmailService _emailService;

		public AuthController(IAuthServices authServices, SignInManager<User> signInManager, UserManager<User> userManager, 
			IHttpContextAccessor httpContextAccessor, IAccountService accountService, IEmailService emailService)
		{
			_authServices = authServices;
			_signInManager = signInManager;
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
			_accountService = accountService;
			_emailService = emailService;
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			ErrorModel errors = new ErrorModel();
			model.Email = model.Email.Trim();
			model.Password = model.Password.Trim();

			try
			{
				var account = await _userManager.FindByEmailAsync(model.Email);
				if (account != null)
				{
					var result = await _signInManager.PasswordSignInAsync(account.UserName, model.Password, false, false);
					if (result.Succeeded)
					{
						var roleAccount = await _userManager.GetRolesAsync(account);
						string roleName = roleAccount.Count() == 0 ? "User" : roleAccount.First();
						int value = (int)Enum.Parse(typeof(Role), roleName);
						return Ok(new
						{
							token = JWTHelper.GenerateJwtToken(account.UserName, account.Id, value),
						});
					}
					else
					{
						if (result.IsNotAllowed)
						{
							errors.Add(ErrorResource.EmailNotConfirm);
						}
						else
						{
							errors.Add(ErrorResource.LoginFail);
						}
						return BadRequest(errors);
					}
				}
				return BadRequest(errors);
			}
			catch (Exception e)
			{
				errors.Add(e.Message.ToString());
				return BadRequest(errors);
			}
		}

		[HttpDelete("Logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			foreach (var cookie in Request.Cookies.Keys)
			{
				Response.Cookies.Delete(cookie);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("ConfirmEmail/{email}/{token}")]
		public async Task<IActionResult> ConfirmEmail(string token, string email)
		{
			ErrorModel errors = new ErrorModel();
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				errors.Add(String.Format(ErrorResource.NotFound, "User"));
				return BadRequest(errors);
			}

			var result = await _userManager.ConfirmEmailAsync(user, token.Replace("@", "/"));
			if (result.Succeeded)
			{
				return Ok(ErrorResource.EmailVerificationSuccessful);
			}
			else
			{
				errors.Errors.Add(ErrorResource.TokenExpried);
				return BadRequest(errors);
			}
		}

		[HttpGet]
		[Route("ResendConfirmEmail/{email}")]
		public async Task<IActionResult> ResendConfirmEmail(string email)
		{
			ErrorModel errors = new ErrorModel();
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null || user.EmailConfirmed)
			{
				errors.Add(String.Format(ErrorResource.NotFound, "User"));
				return BadRequest(errors);
			}

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			BackgroundJob.Enqueue(() => _emailService.SendMailConfirmAsync(new EmailContent()
			{
				Subject = "Xác nhận email",
				To = user.Email
			}, user.FullName, token, user.Email));

			return Ok();
		}

		[HttpGet]
		[Route("SendEmailResetPassword/{email}")]
		public async Task<IActionResult> SendEmailResetPassword(string email)
		{
			ErrorModel errors = new ErrorModel();
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				errors.Add(String.Format(ErrorResource.NotFound, "User"));
				return BadRequest(errors);
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			BackgroundJob.Enqueue(() => _emailService.SendMailResetPasswordAsync(new EmailContent()
			{
				Subject = "Xác nhận email",
				To = user.Email
			}, user.FullName, token, user.Email));

			return Ok();
		}


		[HttpPut("ResetPassword")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
		{
			ErrorModel errors = new ErrorModel();
			if (!ModelState.IsValid)
			{
				AddErrorsFromModelState(ref errors);
				return BadRequest(errors);
			}

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "User"));
			}
			else
			{
				IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token.Replace("@", "/"), model.Password);
				if (result.Succeeded)
				{
					return Ok();
				}
				else
				{
					errors.Errors.Add(ErrorResource.TokenExpried);
				}
			}
			return BadRequest(errors);
		}
	}
}
