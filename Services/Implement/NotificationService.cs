using Golbaus_BE.Commons.Helper;
using Golbaus_BE.DTOs.Notifications;
using Golbaus_BE.DTOs;
using Golbaus_BE.Entities;
using Golbaus_BE.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.ErrorLocalization;

namespace Golbaus_BE.Services.Implement
{
	public class NotificationService : INotificationService
	{
		private readonly ApiDbContext _dbContext;
		private readonly UserResolverService _userResolverService;

		public NotificationService(ApiDbContext dbContext, UserResolverService userResolverService)
		{
			_dbContext = dbContext;
			_userResolverService = userResolverService;
		}

		public PaginationModel<NotificationListModel> GetAllByToken(PaginationNotificationRequest req)
		{
			string userId = _userResolverService.GetUser();
			var notifications = _dbContext.Notifications
								.Include(x => x.Notifier).OrderByDescending(x => x.CreatedDate)
								.Where(x => x.SubscriberId == userId);
			Filter(req, ref notifications);
			return new PaginationModel<NotificationListModel>(req, notifications.Select(x => new NotificationListModel(x)));
		}

		public void MarkAllRead()
		{
			string userId = _userResolverService.GetUser();
			var notifications = _dbContext.Notifications.Where(x => x.SubscriberId == userId && !x.IsRead);
			foreach (var notification in notifications)
			{
				notification.IsRead = true;
			}
			_dbContext.SaveChanges();
		}

		public void MarkRead(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			var notification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id && x.SubscriberId == userId);
			if (notification == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Notification"));
			}
			else
			{
				notification.IsRead = true;
				_dbContext.SaveChanges();
			}
		}

		public void MarkUnread(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			var notification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id && x.SubscriberId == userId);
			if (notification == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Notification"));
			}
			else
			{
				notification.IsRead = false;
				_dbContext.SaveChanges();
			}
		}

		public void Delete(Guid id, ErrorModel errors)
		{
			string userId = _userResolverService.GetUser();
			var notification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id && x.SubscriberId == userId);
			if (notification == null)
			{
				errors.Add(string.Format(ErrorResource.NotFound, "Notification"));
			}
			else
			{
				_dbContext.Notifications.Remove(notification);
				_dbContext.SaveChanges();
			}
		}

		#region Helper

		private void Filter(PaginationNotificationRequest req, ref IQueryable<Notification> data)
		{
			if (req.NotificationDateFrom.HasValue)
			{
				data = data.Where(x => x.CreatedDate >= req.NotificationDateFrom.Value);
			}
			if (req.NotificationDateTo.HasValue)
			{
				data = data.Where(x => x.CreatedDate <= req.NotificationDateTo.Value);
			}
			if (req.Types != null && req.Types.Any())
			{
				data = data.Where(x => req.Types.Contains(x.Type));
			}
			if (req.UnRead)
			{
				data = data.Where(x => !x.IsRead);
			}
			if (req.OrderType.HasValue)
			{
				data = (!req.OrderType.HasValue || req.OrderType == OrderType.Descending) ? data.OrderByDescending(x => x.CreatedDate) : data.OrderBy(x => x.CreatedDate);
			}
		}

		#endregion
	}
}
