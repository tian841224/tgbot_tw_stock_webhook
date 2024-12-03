using System.ComponentModel.DataAnnotations;

namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    public class NotificationHistory : BaseEntity
    {
        [Required]
        public DateTime NotificationTime { get; set; }
    }
}
