﻿using System.Data.SQLite;

namespace HabitTracker.Server.Classes.Habit
{
    public interface IHabitRepository
    {
        void OpenConnection(SQLiteConnection sqlite_conn);
        IEnumerable<Habit> GetAllByUsername(string username);
        Habit? GetById(int id);
        void Add(Habit habit);
        void Update(Habit habit);
        void Delete(int id);
    }
}
