﻿namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    /// <summary>
    /// 使用者
    /// </summary>
    public class User :BaseEntity
    {
        //public override int Id { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        public string? UserName { get; set; } 

        /// <summary>
        /// 是否開啟訂閱
        /// </summary>
        public bool IsSubActive { get; set; }

        public ICollection<Subscription> Subscription { get; set; } = [];
    }
}