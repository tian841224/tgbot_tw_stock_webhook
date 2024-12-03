using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Interface.Repository
{
    public interface IUserRepository
    {
        /// <summary> 使用ID取得資料 </summary>
        Task<User?> GetByIdAsync(int id);

        /// <summary> 使用UserID取得資料 </summary>
        Task<User?> GetByChatIdAsync(long chatId);

        /// <summary> 取得全部資料 </summary>
        Task<List<User>> GetAllAsync();

        /// <summary> 新增資料 </summary>
        Task<int> AddAsync(User user);
    }
}