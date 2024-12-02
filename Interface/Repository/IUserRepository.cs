using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface IUserRepository
    {
        /// <summary> 使用ID取得User </summary>
        Task<User?> GetById(int id);

        /// <summary> 取得全部User </summary>
        Task<List<User>> GetAll();
    }
}