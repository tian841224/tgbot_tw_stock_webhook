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
        public required int UserId { get; set; }

        [Required]
        [MaxLength(10)]
        /// <summary> 股票代號 </summary>
        public required string Symbol { get; set; }

        public virtual User User { get; set; }
    }
}