using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs.Users;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Golbaus_BE.Services.Implement
{
	public class AccountService : IAccountService
	{
		private readonly UserManager<User> _userManager;
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;

		public AccountService(UserManager<User> userManager, ApiDbContext dbContext, UserResolverService userResolverService, IConfiguration configuration)
		{
			_userManager = userManager;
			_dbContext = dbContext;
			_userResolverService = userResolverService;
		}

		public UserGetByTokenModel GetByToken()
		{
			string userId = _userResolverService.GetUser();
			var user = _dbContext.Users.Where(x => x.Id  == userId)
								.Include(x => x.UserRoleMaps).ThenInclude(x => x.Role)
								.FirstOrDefault();
			return new UserGetByTokenModel(user);
		}
	}
}
