using Microsoft.EntityFrameworkCore;
using TGBot_TW_Stock_Webhook.Model.Entities;
using TGBot_TW_Stock_Webhook.Enum;
using System.Reflection;
using TGBot_TW_Stock_Webhook.Model.Configuration;

namespace TGBot_TW_Stock_Webhook.Data
{
    public class AppDbContext(DataBase _dbConfig) : DbContext
    {
        public required DbSet<User> Users { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<NotificationHistory> NotificationHistorys { get; set; } = null!;
        public DbSet<SubscriptionUser> SubscriptionUsers { get; set; } = null!;
        public DbSet<SubscriptionUserStock> SubscriptionUserStocks { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // 判斷使用何者資料庫
            if (_dbConfig?.Type == DbTypeEnum.Sqlite)
            {
                string projectName = Assembly.GetExecutingAssembly().GetName().Name ?? "bot-tw-stock-webhook";
                string savePath = Environment.CurrentDirectory + $"/{projectName}.db";
                options.UseSqlite($"Data Source={savePath}");
            }
            else if (_dbConfig?.Type == DbTypeEnum.MySql)
            {
                options.UseMySql(_dbConfig.ConnectionStrings, ServerVersion.AutoDetect(_dbConfig.ConnectionStrings));
            }
        }

        public override int SaveChanges()
        {
            // 自動更新建立時間與更新時間
            var entries = ChangeTracker.Entries<BaseEntity>();
            var now = DateTime.Now;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                }
            }

            return base.SaveChanges();
        }
    }
}
