using Golbaus_BE.Commons.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golbaus_BE.Entities
{
    public class Notification
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        //Notification information
        public string Content { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public Guid IssueId { get; set; }

        //User received notification
        public string SubscriberId { get; set; }
        //User create issue
        public string NotifierId { get; set; }

        public virtual User Subscriber { get; set; }
        public virtual User Notifier { get; set; }
    }
}
