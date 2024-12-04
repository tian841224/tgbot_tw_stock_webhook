﻿using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Data;
using TGBot_TW_Stock_Webhook.Interface.Repository;
using TGBot_TW_Stock_Webhook.Model.Entities;

namespace TGBot_TW_Stock_Webhook.Repository
{
    public class SubscriptionUserRepository : ISubscriptionUserRepository
    {
        private readonly ILogger<SubscriptionUserRepository> _logger;

        private readonly AppDbContext _context;

        public SubscriptionUserRepository(ILogger<SubscriptionUserRepository> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<SubscriptionUser?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.SubscriptionUsers.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById");
                throw;
            }
        }

        public async Task<SubscriptionUser?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.SubscriptionUsers.FirstOrDefaultAsync(x => x.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByUserId");
                throw;
            }
        }
        public async Task<SubscriptionUser?> GetBySubscriptionIdAsync(int subscriptionId)
        {
            try
            {
                return await _context.SubscriptionUsers.FirstOrDefaultAsync(x => x.SubscriptionId == subscriptionId);
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
                return await _context.SubscriptionUsers.ToListAsync();
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
                await _context.SubscriptionUsers.AddAsync(subscriptionUser);
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