using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ILogger<SubscriptionRepository> _logger;

        private readonly AppDbContext _context;
        public SubscriptionRepository(ILogger<SubscriptionRepository> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Subscription?> GetById(int id)
        {
            try
            {
                return await _context.Subscriptions.FirstOrDefaultAsync(x => x.IsDelete == false && x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<List<Subscription>?> GetByUserId(int userId)
        {
            try
            {
                return await _context.Subscriptions.Where(x => x.IsDelete == false && x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByUserId");
                throw;
            }
        }


        public async Task<List<Subscription>> GetAll()
        {
            try
            {
                return await _context.Subscriptions.Where(x => x.IsDelete == false).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll");
                throw;
            }
        }
    }
}