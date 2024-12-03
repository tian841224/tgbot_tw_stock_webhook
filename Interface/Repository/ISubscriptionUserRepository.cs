using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface ISubscriptionUserRepository
    {
        /// <summary> 使用Id取得資料 </summary>
        Task<SubscriptionUser?> GetById(int id);

        /// <summary> 使用UserId取得資料 </summary>
        Task<SubscriptionUser?> GetByUserId(int userId);

        /// <summary> 使用SubscriptionId取得資料 </summary>
        Task<SubscriptionUser?> GetBySubscriptionId(int subscriptionId);

        /// <summary> 取得全部Subscription </summary>
        Task<List<SubscriptionUser>> GetAll();
    }
}
