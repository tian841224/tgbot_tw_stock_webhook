using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    /// <summary>
    /// 訂閱股票功能
    /// </summary>
    public class SubscriptionUserStock : BaseEntity
    {
        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [Required]
        /// <summary> 股票代號 </summary>
        public required string Symbol { get; set; }

        public required virtual User User { get; set; }
    }
}