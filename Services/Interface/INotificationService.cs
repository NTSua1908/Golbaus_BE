using Golbaus_BE.DTOs;
using Golbaus_BE.DTOs.Notifications;

namespace Golbaus_BE.Services.Interface
{
	public interface INotificationService
	{
		PaginationModel<NotificationListModel> GetAllByToken(PaginationNotificationRequest req);
		void MarkAllRead();
		void MarkRead(Guid id, ErrorModel errors);
		void MarkUnread(Guid id, ErrorModel errors);
		void Delete(Guid id, ErrorModel errors);
	}
}
