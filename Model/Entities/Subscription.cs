using System.ComponentModel.DataAnnotations.Schema;

namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    /// <summary>
    /// 訂閱清單
    /// </summary>
    public class Subscription : BaseEntity
    {
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string Stock { get; set; }

        public User User { get; set; }
    }
}
