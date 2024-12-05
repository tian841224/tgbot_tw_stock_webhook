using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Enum;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class SubscriptionRepository(ILogger<SubscriptionRepository> _logger, IDbContextFactory<AppDbContext> _contextFactory) : ISubscriptionRepository
    {
        public async Task<Subscription?> GetByIdAsync(int id)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.Subscriptions.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<Subscription?> GetByItemAsync(SubscriptionItemEnum item)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.Subscriptions.AsNoTracking().FirstOrDefaultAsync(x => x.Item == item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByItem");
                throw;
            }
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.Subscriptions.AsNoTracking().Where(x => x.Status == true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll");
                throw;
            }
        }

        public async Task<int> AddAsync(Subscription subscription)
        {
            try
            {
                using var _context = await _contextFactory.CreateDbContextAsync();
                await _context.Subscriptions.AddAsync(subscription);
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