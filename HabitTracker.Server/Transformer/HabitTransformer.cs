using HabitTracker.Server.DTOs;
using System.Data;

namespace HabitTracker.Server.Transformer
{
    public class HabitTransformer : ITransformer<Habit, IDataReader>
    {
        public HabitTransformer() { }

        public Habit Transform(IDataReader reader)
        {
            return new Habit
                (
                    Convert.ToInt32(reader["Id"]),
                    Convert.ToInt32(reader["User_id"]),
                    Convert.ToString(reader["Name"])
                );
        }
    }
}
