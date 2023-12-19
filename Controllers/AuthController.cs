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

		public AuthController(IAuthServices authServices, SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IAccountService accountService)
		{
			_authServices = authServices;
			_signInManager = signInManager;
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
			_accountService = accountService;
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			ErrorModel errors = new ErrorModel();

			try
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
				var account = await _userManager.FindByNameAsync(model.Email);
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
	}
}
