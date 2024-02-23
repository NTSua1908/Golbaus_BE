using Hangfire.Dashboard;

namespace Golbaus_BE.Commons.Helper
{
	public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize(DashboardContext context)
		{
			return true;
		}
	}
}
