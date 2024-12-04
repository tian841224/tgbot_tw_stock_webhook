using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface ISubscriptionUserRepository
    {
        /// <summary> 使用Id取得資料 </summary>
        Task<SubscriptionUser?> GetByIdAsync(int id);

        /// <summary> 使用UserId取得資料 </summary>
        Task<List<SubscriptionUser>?> GetByUserIdAsync(int userId);

        /// <summary> 使用SubscriptionId取得資料 </summary>
        Task<List<SubscriptionUser>?> GetBySubscriptionIdAsync(int subscriptionId);

        /// <summary> 使用UserId和SubscriptionId取得資料 </summary>
        Task<SubscriptionUser?> GetByUserIdAndSubscriptionIdAsync(int userId, int subscriptionId);

        /// <summary> 取得全部資料 </summary>
        Task<List<SubscriptionUser>> GetAllAsync();

        /// <summary> 新增資料 </summary>
        Task<int> AddAsync(SubscriptionUser subscriptionUser);
    }
}
