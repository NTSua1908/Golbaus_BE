using Golbaus_BE.Commons.Constants;

namespace Golbaus_BE.DTOs
{
	public class PaginationRequest
	{
		public int Page { get; set; }
		public int Amount { get; set; }
		public string Sort { get; set; }
		public string SearchText { get; set; }
		public bool All { get; set; } = false;
		public void Format()
		{
			if (string.IsNullOrEmpty(Sort))
			{
				Sort = "";
			}

			if (string.IsNullOrEmpty(SearchText))
			{
				SearchText = "";
			}

			if (Amount <= 0)
			{
				Amount = 10;
			}

			if (Page <= 0)
			{
				Page = 0;
			}
		}
	}

	public class PaginationPostQuestionRequest : PaginationRequest 
	{
        public List<string> Tags { get; set; }
        public DateTime? PublishDateFrom { get; set; }
        public DateTime? PublishDateTo { get; set; }
        public OrderBy? OrderBy { get; set; }
        public OrderType? OrderType { get; set; }
    }

	public class PaginationNotificationRequest : PaginationRequest
	{
		public DateTime? NotificationDateFrom { get; set; }
		public DateTime? NotificationDateTo { get; set; }
        public List<NotificationType> Types { get; set; }
        public bool UnRead { get; set; }
        public OrderType? OrderType { get; set; }
	}
}
