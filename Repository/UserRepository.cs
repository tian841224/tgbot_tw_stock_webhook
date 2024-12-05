using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class UserRepository(ILogger<UserRepository> _logger, IDbContextFactory<AppDbContext> _contextFactory) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<User?> GetByChatIdAsync(long chatId)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.TelegramChatId == chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.Users.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll");
                throw;
            }
        }

        public async Task<int> AddAsync(User user)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                await _context.Users.AddAsync(user);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add");
                throw;
            }
        }
    }
}