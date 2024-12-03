using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface ISubscriptionRepository
    {
        /// <summary> 使用ID取得資料 </summary>
        Task<Subscription?> GetByIdAsync(int id);

        /// <summary> 使用Item取得資料 </summary>
        Task<Subscription?> GetByItemAsync(SubscriptionItemEnum item);

        /// <summary> 取得全部資料 </summary>
        Task<List<Subscription>> GetAllAsync();

        /// <summary> 新增資料 </summary>
        Task<int> AddAsync(Subscription subscription);
    }
}