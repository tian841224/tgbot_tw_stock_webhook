using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Repository
{
    public class SubscriptionUserStockRepository : ISubscriptionUserStockRepository
    {
        private readonly ILogger<SubscriptionUserStockRepository> _logger;

        private readonly AppDbContext _context;

        public SubscriptionUserStockRepository(ILogger<SubscriptionUserStockRepository> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<SubscriptionUserStock?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.SubscriptionUserStocks.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<List<SubscriptionUserStock>?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.SubscriptionUserStocks.Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByUserId");
                throw;
            }
        }

        public async Task<List<SubscriptionUserStock>?> GetBySymbolAsync(string symbol)
        {
            try
            {
                return await _context.SubscriptionUserStocks.Where(x => x.Symbol == symbol).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBySymbol");
                throw;
            }
        }

        public async Task<SubscriptionUserStock?> GetByUserIdAndSymbolAsync(int userId, string symbol)
        {
            try
            {
                return await _context.SubscriptionUserStocks.FirstOrDefaultAsync(x => x.UserId == userId && x.Symbol == symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByUserIdAndSymbol");
                throw;

            }
        }

        public async Task<List<SubscriptionUserStock>> GetAllAsync()
        {
            try
            {
                return await _context.SubscriptionUserStocks.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll");
                throw;
            }
        }

        public async Task<int> AddAsync(SubscriptionUserStock subscriptionUserStock)
        {
            try
            {
                await _context.SubscriptionUserStocks.AddAsync(subscriptionUserStock);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add");
                throw;
            }
        }

        public async Task<int> DeleteAsync(SubscriptionUserStock subscriptionUserStock)
        {
            try
            {
                _context.SubscriptionUserStocks.Remove(subscriptionUserStock);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete");
                throw;
            }
        }
    }
}
