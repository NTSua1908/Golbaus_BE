namespace Golbaus_BE.DTOs
{
	public class PaginationModel<T>
	{
		public List<T> Data { get; set; }
		public int Page { get; set; }
		public int Amount { get; set; }
		public int TotalPage { get; set; }
		public string Sort { get; set; }
		public string SearchText { get; set; }
		public long TotalCount { get; set; }
		public PaginationModel()
		{
			Data = new List<T>();
		}
		public PaginationModel(PaginationRequest request, IEnumerable<T> list)
		{

			Sort = request.Sort;
			Page = request.Page;
			SearchText = request.SearchText;
			if (request.All)
			{
				Data = list.ToList();
				Amount = Data.Count == 0 ? 1 : Data.Count;
				TotalCount = Data.Count;
				TotalPage = 1;
			}
			else
			{
				Amount = request.Amount == 0 ? 1 : request.Amount;
				TotalCount = list.Count();
				TotalPage = (int)Math.Ceiling((decimal)this.TotalCount / Amount);
				Data = list.Skip(Amount * Page).Take(Amount).ToList();
			}
		}
	}
}
