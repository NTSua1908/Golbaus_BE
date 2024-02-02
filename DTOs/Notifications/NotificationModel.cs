using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Entities;

namespace Golbaus_BE.DTOs.Notifications
{
	public class NotificationListModel
	{
		public Guid Id { get; set; }
		public string Content { get; set; }
		public NotificationType Type { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsRead { get; set; }
		public Guid IssueId { get; set; }
        public string UserId { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }

		public NotificationListModel(Notification notification)
		{
			Id = notification.Id;
			Content = notification.Content;
			Type = notification.Type;
			CreatedDate = notification.CreatedDate;
			IsRead = notification.IsRead;
			IssueId = notification.IssueId;
			UserId = notification.Notifier.Id;
			UserName = notification.Notifier.UserName;
			Avatar = notification.Notifier.Avatar;
		}
    }
}
