﻿namespace HabitTracker.Server.DTOs
{
    public class Habit
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public string Name { get; set; }

        public Habit(
                int id,
                int user_id,
                string name
            )
        {
            Id = id;
            User_id = user_id;
            Name = name;
        }
    }
}