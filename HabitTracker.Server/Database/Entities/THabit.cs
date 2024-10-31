using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Database.Entities
{
    public class THabit
    {
        [Key]
        public int Id {  get; set; }
        public int User_id { get; set; }
        public string Name { get; set; }
        public TUser User { get; set; }
        public List<THabitLog> HabitLogs { get; set; }
    }
}
