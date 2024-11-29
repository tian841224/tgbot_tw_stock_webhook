using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;

        private readonly AppDbContext _context;
        public UserRepository(ILogger<UserRepository> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<User?> GetById(int id)
        {
            try
            {
                return await _context.Users.Where(x => x.Status == true && x.IsDelete == false && x.SubActive == true && x.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<List<User>> GetAll()
        {
            try
            {
                return await _context.Users.Where(x => x.Status == true && x.IsDelete == false && x.SubActive == true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll");
                throw;
            }
        }
    }
}