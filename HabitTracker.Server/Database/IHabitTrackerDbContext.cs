using Microsoft.EntityFrameworkCore;
using HabitTracker.Server.Database.Entities;
using Microsoft.EntityFrameworkCore.Internal;

namespace HabitTracker.Server.Database
{
    public interface IHabitTrackerDbContext
    {
        DbSet<TUser> Users { get; set; }
        DbSet<THabit> Habits { get; set; }
        DbSet<THabitLog> HabitLogs { get; set; }

        DbSet<TRate> Rates { get; set; }
        int SaveChanges();
    }
}
