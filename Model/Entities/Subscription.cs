using System.ComponentModel.DataAnnotations.Schema;

namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    /// <summary>
    /// 訂閱清單
    /// </summary>
    public class Subscription : BaseEntity
    {
        /// <summary>
        /// 順序編號
        /// </summary>
        public uint Serial { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        /// <summary>
        /// 股票代號
        /// </summary>
        public required string Symbol{ get; set; }
        /// <summary>
        /// 股票名稱
        /// </summary>
        public required string Name { get; set; } 

        public User? User { get; set; }
    }
}
