using System.Data;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Transformer
{
    public class UserTransformer : ITransformer<IReadOnlyCollection<User>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>
    {
        public UserTransformer() { }

        public IReadOnlyCollection<User> Transform(IReadOnlyCollection<IReadOnlyDictionary<string, object>> data)
        {
            List<User> users = new List<User>();

            foreach (IReadOnlyDictionary<string, object> d in data)
            {
                users.Add(new User(
                    Convert.ToInt32(d["Id"]),
                    (string)d["Username"],
                    (string)d["Email"],
                    (string)d["Password"]
                ));
            }

            return users;
        }
    }
}
