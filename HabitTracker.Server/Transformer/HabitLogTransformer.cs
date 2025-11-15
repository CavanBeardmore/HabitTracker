using HabitTracker.Server.DTOs;
using System.Text.Json;

namespace HabitTracker.Server.Transformer
{
    public class HabitLogTransformer : ITransformer<IReadOnlyCollection<HabitLog>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>
    {
        public HabitLogTransformer() { }

        public IReadOnlyCollection<HabitLog> Transform(IReadOnlyCollection<IReadOnlyDictionary<string, object>> data)
        {
            List<HabitLog> habitLogs = new List<HabitLog>();

            foreach (IReadOnlyDictionary<string, object> d in data)
            {
                Console.WriteLine($"HABIT LOG {JsonSerializer.Serialize(d)}");
                habitLogs.Add(new HabitLog
                (
                    Convert.ToInt32(d["Id"]),
                    Convert.ToInt32(d["Habit_id"]),
                    Convert.ToDateTime(d["Start_date"]),
                    Convert.ToBoolean(d["Habit_logged"]),
                    Convert.ToInt32(d["Length_in_days"])
                ));
            }

            return habitLogs;
        }
    }
}
