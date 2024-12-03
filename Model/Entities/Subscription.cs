using System.ComponentModel.DataAnnotations;
using TGBot_TW_Stock_Webhook.Enum;

namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    /// <summary>
    /// 訂閱項目
    /// </summary>
    public class Subscription : BaseEntity
    {
        [Required]
        /// <summary> 訂閱項目類型 </summary>
        public required SubscriptionItemEnum Item { get; set; }

        /// <summary> 訂閱項目描述 </summary>
        public string? Description { get; set; }

        /// <summary> 啟用狀態 </summary>
        public bool Status { get; set; } = true;
    }
}
