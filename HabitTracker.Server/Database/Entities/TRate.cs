using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.Database.Entities
{
    public class TRate
    {
        [Key]
        public string IpAddress { get; set; }
        public uint Count {  get; set; }
        public DateTime Ttl { get; set; }
    }
}
