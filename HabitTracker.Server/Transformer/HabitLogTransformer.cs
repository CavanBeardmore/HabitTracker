using HabitTracker.Server.DTOs;
using System.Data;

namespace HabitTracker.Server.Transformer
{
    public class HabitLogTransformer : ITransformer<HabitLog, IDataReader>
    {
        public HabitLogTransformer() { }

        public HabitLog Transform(IDataReader reader)
        {
            return new HabitLog
            (
                Convert.ToInt32(reader["Id"]),
                Convert.ToInt32(reader["Habit_id"]),
                Convert.ToDateTime(reader["Start_date"]),
                Convert.ToBoolean(reader["Habit_logged"]),
                Convert.ToInt32(reader["Length_in_days"])
            );
        }
    }
}
