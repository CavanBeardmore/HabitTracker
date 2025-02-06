using HabitTracker.Server.DTOs;
using System.Data;

namespace HabitTracker.Server.Transformer
{
    public class HabitTransformer : ITransformer<IReadOnlyCollection<Habit>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>
    {
        public HabitTransformer() { }

        public IReadOnlyCollection<Habit> Transform(IReadOnlyCollection<IReadOnlyDictionary<string, object>> data)
        {
            List<Habit> habits = new List<Habit>();

            foreach (IReadOnlyDictionary<string, object> d in data)
            {

                habits.Add(new Habit(
                        Convert.ToInt32(d["Id"]),
                        Convert.ToInt32(d["User_id"]),
                        (string)d["Name"]
                ));
            }

            return habits;
        }
    }
}
