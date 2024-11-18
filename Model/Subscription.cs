using System.ComponentModel.DataAnnotations.Schema;

namespace TGBot_TW_Stock_Webhook.Model
{
    /// <summary>
    /// 訂閱清單
    /// </summary>
    public class Subscription
    {
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string Stock { get; set; }

        public User User { get; set; }
    }
}
