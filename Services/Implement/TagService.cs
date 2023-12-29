using Golbaus_BE.Commons.Helper;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;

namespace Golbaus_BE.Services.Implement
{
	public class TagService : ITagService
	{
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;

		public TagService(ApiDbContext dbContext, UserResolverService userResolverService)
		{
			_dbContext = dbContext;
			_userResolverService = userResolverService;
		}

		public List<string> GetTagNotExist(List<string> tags)
		{
			tags = tags.Select(x => x.ToLower()).ToList();
			var existedTags = _dbContext.Tags.Where(x => tags.Contains(x.Name.ToLower())).Select(x => x.Name).ToList();
			return tags.Except(existedTags).ToList();
		}
	}
}
