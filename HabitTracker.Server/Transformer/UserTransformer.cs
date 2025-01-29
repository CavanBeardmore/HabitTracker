using System.Data;
using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Transformer
{
    public class UserTransformer : ITransformer<User, IDataReader>
    {
        public UserTransformer() { }

        public User Transform(IDataReader dataReader)
        {
            return new User(
                    Convert.ToInt32(dataReader["Id"]),
                    (string)dataReader["Username"],
                    (string)dataReader["Email"],
                    (string)dataReader["Password"]
                );
        }
    }
}
