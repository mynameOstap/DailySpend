using DailySpendServer.Model;
using Microsoft.EntityFrameworkCore;

namespace DailySpendServer.Data
{
    public class DailySpendContext : DbContext
    {
        public DailySpendContext(DbContextOptions<DailySpendContext> options) : base(options)
        { }

        public DbSet<UserSetting> UserSettings => Set<UserSetting>();
        public DbSet<DailyPlan> DailyPlans => Set<DailyPlan>();
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserSetting>().HasKey(x => x.id);
            modelBuilder.Entity<BankAccount>().HasKey(x => x.id);
            modelBuilder.Entity<DailyPlan>().HasKey(x => x.id);

            modelBuilder.Entity<UserSetting>()
            .HasOne(u => u.BankAccount)
            .WithOne(b => b.UserSetting)
            .HasForeignKey<BankAccount>(b => b.UserSettingId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSetting>()
            .HasMany(u => u.DailyPlans)
            .WithOne(p => p.UserSetting)
            .HasForeignKey(p => p.UserSettingId)
            .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<UserSetting>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.UserSetting)
            .HasForeignKey(n => n.UserSettingId)
            .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
