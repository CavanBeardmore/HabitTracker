namespace HabitTracker.Server.DTOs
{
    public class Rate
    {
        public string IpAddress { get; set; }
        public uint Count { get; set; }
        public DateTime Ttl { get; set; }

        public Rate(string ipAddress, uint count, DateTime ttl) 
        {
            IpAddress = ipAddress;
            Count = count;
            Ttl = ttl;
        }
    }
}
