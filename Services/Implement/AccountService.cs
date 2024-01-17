using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.ErrorLocalization;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Users;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Role = Golbaus_BE.Commons.Constants.Role;

namespace Golbaus_BE.Services.Implement
{
	public class AccountService : IAccountService
	{
		private readonly UserManager<User> _userManager;
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;
		private readonly IEmailService _emailService;

		public AccountService(UserManager<User> userManager, ApiDbContext dbContext, UserResolverService userResolverService, IEmailService emailService)
		{
			_userManager = userManager;
			_dbContext = dbContext;
			_userResolverService = userResolverService;
			_emailService = emailService;
		}

		public async Task<ErrorModel> CreateUser(CreateUserModel model, string hostName)
		{
			ErrorModel errors = new ErrorModel();
			if (ValidateCreatUser(model, errors))
			{
				string password = HashPassword(model.Password);
				User user = model.ParseToEntity(password);
				var result = await _userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					await _userManager.AddToRolesAsync(user, new List<string> { Role.User.ToString() });
					var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					BackgroundJob.Enqueue(() => _emailService.SendMailConfirmAsync(new EmailContent()
					{
						Subject = "Xác nhận email",
						To = user.Email
					}, hostName, user.FullName, token, user.Email));
				}
			}
			return errors;
		}

		public UserGetByTokenModel GetByToken()
		{
			string userId = _userResolverService.GetUser();
			var user = _dbContext.Users.Where(x => x.Id  == userId)
								.Include(x => x.UserRoleMaps).ThenInclude(x => x.Role)
								.FirstOrDefault();
			return new UserGetByTokenModel(user);
		}

		#region Helper

		private bool ValidateCreatUser(CreateUserModel model, ErrorModel errors) 
		{
			FormatCreatUserModel(model);
			var user = _dbContext.Users.Where(x => x.NormalizedEmail == model.Email.ToUpper() || x.NormalizedUserName == model.UserName.ToUpper());
			if (user != null)
			{
				if (user.Any(x => x.NormalizedEmail == model.Email.ToUpper()))
				{
					errors.Add(string.Format(ErrorResource.AlreadyExists, "Email"));
				}
				if (user.Any(x => x.NormalizedUserName == model.UserName.ToUpper()))
				{
					errors.Add(string.Format(ErrorResource.AlreadyExists, "UserName"));
				}
			}
			else if (!string.Equals(model.Password, model.ConfirmPassword))
			{
				errors.Add(ErrorResource.PasswordNotMatch);
			}

			return errors.IsEmpty;
		}

		private string HashPassword(string password)
		{
			PasswordHasher<User> hasher = new PasswordHasher<User>();
			return hasher.HashPassword(null, password);
		}

		private void FormatCreatUserModel(CreateUserModel model)
		{
			model.UserName = model.UserName.Trim();
			model.Email = model.Email.Trim();
			model.FullName = model.FullName.Trim();
			model.Password = model.Password.Trim();
			model.ConfirmPassword = model.ConfirmPassword.Trim();
		}

		#endregion
	}
}
