using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TGBot_TW_Stock_Webhook.Enum;

namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    /// <summary>
    /// 使用者訂閱項目
    /// </summary>
    public class SubscriptionUser : BaseEntity
    {
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [ForeignKey("SubscriptionId")]
        public int SubscriptionId { get; set; }

        public required virtual User User { get; set; }
        public required virtual Subscription Subscription { get; set; }
    }
}