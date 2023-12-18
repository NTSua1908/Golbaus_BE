using System.Security.Claims;

namespace Golbaus_BE.Commons.Helper
{
	public class UserResolverService
	{
		private readonly IHttpContextAccessor _context;
		public UserResolverService(IHttpContextAccessor context)
		{
			_context = context;
		}

		public string GetUser()
		{
			try
			{
				var claims = _context.HttpContext?.User?.Claims;

				if (claims!.Count() > 0)
				{
					var nameId = claims!.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

					if (nameId != null)
					{
						return nameId.Value;
					}
				}

				return _context.HttpContext?.User?.Identity?.Name!;
			}
			catch (Exception) { }

			return string.Empty;
		}
	}
}
