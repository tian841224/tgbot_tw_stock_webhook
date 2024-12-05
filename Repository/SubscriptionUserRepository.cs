using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Repository
{
    public class SubscriptionUserRepository(ILogger<SubscriptionUserRepository> _logger, IDbContextFactory<AppDbContext> _contextFactory) : ISubscriptionUserRepository
    {
        public async Task<SubscriptionUser?> GetByIdAsync(int id)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.SubscriptionUsers.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<List<SubscriptionUser>?> GetByUserIdAsync(int userId)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.SubscriptionUsers.AsNoTracking().Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByUserId");
                throw;
            }
        }
        public async Task<List<SubscriptionUser>?> GetBySubscriptionIdAsync(int subscriptionId)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.SubscriptionUsers.AsNoTracking().Where(x => x.SubscriptionId == subscriptionId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBySubscriptionId");
                throw;
            }
        }

        public async Task<SubscriptionUser?> GetByUserIdAndSubscriptionIdAsync(int userId,int subscriptionId)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.SubscriptionUsers.AsNoTracking().FirstOrDefaultAsync(x => x.SubscriptionId == subscriptionId && x.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBySubscriptionId");
                throw;
            }
        }

        public async Task<List<SubscriptionUser>> GetAllAsync()
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.SubscriptionUsers.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll");
                throw;
            }
        }

        public async Task<int> AddAsync(SubscriptionUser subscriptionUser)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                await _context.SubscriptionUsers.AddAsync(subscriptionUser);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add");
                throw;
            }
        }

        public async Task<int> DeleteAsync(SubscriptionUser subscriptionUser)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                _context.SubscriptionUsers.Remove(subscriptionUser);
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
