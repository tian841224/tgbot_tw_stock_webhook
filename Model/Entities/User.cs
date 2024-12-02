namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    /// <summary>
    /// 使用者
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary> 暱稱 </summary>
        public string? UserName { get; set; }

        /// <summary> 是否被停用 </summary>
        public bool Status { get; set; } = true;

        public required virtual ICollection<Subscription> Subscription { get; set; }
    }
}
