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

		public async Task<ErrorModel> CreateUser(CreateUserModel model)
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
					}, user.FullName, token, user.Email));
				}
			}
			return errors;
		}

		public UserGetByTokenModel GetByToken()
		{
			string userId = _userResolverService.GetUser();
			var user = _dbContext.Users.Where(x => x.Id == userId && !x.IsDeleted)
								.Include(x => x.UserRoleMaps).ThenInclude(x => x.Role)
								.FirstOrDefault();
			if (user != null)
			{
				return new UserGetByTokenModel(user);
			}
			return new UserGetByTokenModel();
		}

		public GetDetailModel GetDetailByToken(ErrorModel errors)
		{
			if (ValidateUser(_userResolverService.GetUser(), errors, out User user))
			{
				return new GetDetailModel(user);
			}
			return new GetDetailModel();
		}

		public GetUserProfileModel GetDetailById(string userId, ErrorModel errors)
		{
			string viewerId = _userResolverService.GetUser();

			GetUserProfileModel user = _dbContext.Users
				.Include(x => x.Posts).Include(x => x.Questions)
				.Include(x => x.UserFollowerMaps)
				.Include(x => x.UserFollowingMaps)
				.Where(x => x.Id == userId && !x.IsDeleted)
				.Select(x => new GetUserProfileModel(x, viewerId))
				.FirstOrDefault();
			if (user == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "User"));
			}
			else
			{
				return user;
			}
			return new GetUserProfileModel();
		}

		public void UpdateByToken(UserUpdateByTokenModel model, ErrorModel errors)
		{
			if (ValidateUpdateByToken(model, errors, out User user))
			{
				string password = string.IsNullOrEmpty(model.Password) ? "" : HashPassword(model.Password);
				model.UpdateEntity(user, password);
				_dbContext.SaveChanges();
			}
		}

		public void UpdateAvatarByToken(UpdateAvatarModel model, ErrorModel errors)
		{
			if (ValidateUser(_userResolverService.GetUser(), errors, out User user))
			{
				user.Avatar = model.Avatar;
				_dbContext.SaveChanges();
			}
		}

		public void ToggleFollow(string userId, ErrorModel errors)
		{
			string followerId = _userResolverService.GetUser();
			if (ValidateFollow(followerId, userId, errors))
			{
				var follow = _dbContext.UserFollowMaps.FirstOrDefault(x => x.FollowerId == followerId && x.FollowingId == userId);
				if (follow == null)
				{
					_dbContext.UserFollowMaps.Add(new UserFollowMap()
					{
						FollowerId = followerId,
						FollowingId = userId
					});
				}
				else
				{
					_dbContext.UserFollowMaps.Remove(follow);
				}
				_dbContext.SaveChanges();
			}
		}

		#region Helper

		private bool ValidateCreatUser(CreateUserModel model, ErrorModel errors)
		{
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

		private bool ValidateUpdateByToken(UserUpdateByTokenModel model, ErrorModel errors, out User user)
		{
			if (ValidateUser(_userResolverService.GetUser(), errors, out user))
			{
				if (!string.IsNullOrEmpty(model.Password) && !string.Equals(model.Password, model.ConfirmPassword))
				{
					errors.Add(ErrorResource.PasswordNotMatch);
				}
			}

			return errors.IsEmpty;
		}

		private string HashPassword(string password)
		{
			PasswordHasher<User> hasher = new PasswordHasher<User>();
			return hasher.HashPassword(null, password);
		}

		private bool ValidateUser(string userId, ErrorModel errors, out User user)
		{
			user = _dbContext.Users.FirstOrDefault(x => x.Id == userId && !x.IsDeleted);
			if (user == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "User"));
			}
			return errors.IsEmpty;
		}

		private bool ValidateFollow(string followerId, string followedId, ErrorModel errors)
		{
			if (followerId == followedId)
			{
				errors.Add(string.Format(ErrorResource.Invalid, "User"));
			}

			ValidateUser(followerId, errors, out User user);
			ValidateUser(_userResolverService.GetUser(), errors, out User following);

			return errors.IsEmpty;
		}

		#endregion
	}
}
