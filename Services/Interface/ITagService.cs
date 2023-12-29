namespace Golbaus_BE.Services.Interface
{
	public interface ITagService
	{
		List<string> GetTagNotExist(List<string> tags);
	}
}
