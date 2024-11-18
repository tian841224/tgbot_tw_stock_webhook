namespace TGBot_TW_Stock_Webhook.Model
{
    /// <summary>
    /// 使用者
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        /// <summary>
        /// 是否開啟訂閱
        /// </summary>
        public bool IsSubActive { get; set; }

        /// <summary>
        /// 是否停用帳號
        /// </summary>
        public bool IsDelete { get; set; }

        public ICollection<Subscription> Subscription { get; set; } = [];
    }
}
