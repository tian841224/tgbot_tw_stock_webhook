using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface ISubscriptionUserStockRepository
    {
        /// <summary> 使用Id取得資料 </summary>
        Task<SubscriptionUserStock?> GetById(int id);

        /// <summary> 使用UserId取得資料 </summary>
        Task<SubscriptionUserStock?> GetByUserId(int userId);

        /// <summary> 使用Symbol取得資料 </summary>
        Task<SubscriptionUserStock?> GetBySymbol(string symbol);

        /// <summary> 取得全部Subscription </summary>
        Task<List<SubscriptionUserStock>> GetAll();
    }
}
