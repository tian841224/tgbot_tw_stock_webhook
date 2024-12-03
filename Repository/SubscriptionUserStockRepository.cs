using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Repository
{
    public class SubscriptionUserStockRepository: ISubscriptionUserStockRepository
    {
        private readonly ILogger<SubscriptionUserStockRepository> _logger;

        private readonly AppDbContext _context;

        public SubscriptionUserStockRepository(ILogger<SubscriptionUserStockRepository> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<SubscriptionUserStock?> GetById(int id)
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

        public async Task<SubscriptionUserStock?> GetByUserId(int userId)
        {
            try
            {
                return await _context.SubscriptionUserStocks.FirstOrDefaultAsync(x => x.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByUserId");
                throw;
            }
        }
        public async Task<SubscriptionUserStock?> GetBySymbol(string symbol)
        {
            try
            {
                return await _context.SubscriptionUserStocks.FirstOrDefaultAsync(x => x.Symbol == symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBySymbol");
                throw;
            }
        }

        public async Task<List<SubscriptionUserStock>> GetAll()
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
    }
}
