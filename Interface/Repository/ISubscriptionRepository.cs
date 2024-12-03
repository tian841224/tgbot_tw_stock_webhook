using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface ISubscriptionRepository
    {
        /// <summary> 使用ID取得資料 </summary>
        Task<Subscription?> GetById(int id);

        /// <summary> 使用UserId取得資料 </summary>
        Task<List<Subscription>?> GetByItem(SubscriptionItemEnum item);

        /// <summary> 取得全部資料 </summary>
        Task<List<Subscription>> GetAll();
    }
}