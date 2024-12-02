using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface ISubscriptionRepository
    {
        /// <summary> 使用ID取得Subscription </summary>
        Task<Subscription?> GetById(int id);
        /// <summary> 使用UserId取得Subscription </summary>
        Task<List<Subscription>?> GetByUserId(int userId);

        /// <summary> 取得全部Subscription </summary>
        Task<List<Subscription>> GetAll();
    }
}