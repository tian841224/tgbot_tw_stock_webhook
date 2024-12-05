using TGBot_TW_Stock_Webhook.Enum;

namespace TGBot_TW_Stock_Webhook.Model
{
    public sealed class DataBase
    {
        public DbTypeEnum Type { get; set; }
        public required string ConnectionStrings { get; set; }
    }
}
