using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface ISubscriptionUserStockRepository
    {
        /// <summary> 使用Id取得資料 </summary>
        Task<SubscriptionUserStock?> GetByIdAsync(int id);

        /// <summary> 使用UserId取得資料 </summary>
        Task<List<SubscriptionUserStock>?> GetByUserIdAsync(int userId);

        /// <summary> 使用Symbol取得資料 </summary>
        Task<List<SubscriptionUserStock>?> GetBySymbolAsync(string symbol);

        /// <summary> 使用UserId和Symbol取得資料 </summary>
        Task<SubscriptionUserStock?> GetByUserIdAndSymbolAsync(int userId, string symbol);

        /// <summary> 取得全部資料 </summary>
        Task<List<SubscriptionUserStock>> GetAllAsync();

        /// <summary> 新增資料 </summary>
        Task<int> AddAsync(SubscriptionUserStock subscriptionUserStock);

        /// <summary> 刪除資料 </summary>
        Task<int> DeleteAsync(SubscriptionUserStock subscriptionUserStock);
    }
}
