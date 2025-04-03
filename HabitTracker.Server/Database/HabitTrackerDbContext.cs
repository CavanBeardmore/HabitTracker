using Microsoft.EntityFrameworkCore;
using HabitTracker.Server.Database.Entities;

namespace HabitTracker.Server.Database
{
    public class HabitTrackerDbContext: DbContext, IHabitTrackerDbContext
    {   
        public DbSet<TUser> Users { get; set; }
        public DbSet<THabit> Habits { get; set; }
        public DbSet<THabitLog> HabitLogs { get; set; }
        public DbSet<TRate> Rates { get; set; }

        public HabitTrackerDbContext(DbContextOptions<HabitTrackerDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\Users\cavan\OneDrive\Desktop\HabitTracker\Habit-DB.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<THabit>()
            .Property(h => h.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<THabitLog>()
            .Property(hl => hl.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<TUser>()
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<TUser>()
             .Property(t => t.IsDeleted)
             .HasDefaultValue(false);

            modelBuilder.Entity<TUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

            modelBuilder.Entity<THabit>()
                .HasOne(h => h.User)
                .WithMany(u => u.Habits)
                .HasForeignKey(h => h.User_id);

            modelBuilder.Entity<THabitLog>()
                .HasOne(hl => hl.Habit)
                .WithMany(h => h.HabitLogs)
                .HasForeignKey(hl => hl.Habit_id);
        }
    }
}
