using System.ComponentModel.DataAnnotations;

namespace TGBot_TW_Stock_Webhook.Model.Entities
{
    public class BaseEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [Required]
        public virtual int Id { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新時間
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 刪除狀態
        /// </summary>
        [Required]
        public bool IsDelete { get; set; } = false;
    }
}