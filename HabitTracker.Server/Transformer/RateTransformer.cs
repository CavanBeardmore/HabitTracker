using HabitTracker.Server.DTOs;

namespace HabitTracker.Server.Transformer
{
    public class RateTransformer : ITransformer<IReadOnlyCollection<Rate>, IReadOnlyCollection<IReadOnlyDictionary<string, object>>>
    {
        public RateTransformer() { }

        public IReadOnlyCollection<Rate> Transform(IReadOnlyCollection<IReadOnlyDictionary<string, object>> data)
        {
            List<Rate> rates = new List<Rate>();

            foreach (IReadOnlyDictionary<string, object> d in data)
            {

                Console.WriteLine($"TTL {Convert.ToDateTime(d["Ttl"]).ToUniversalTime()}");
                rates.Add(
                        new Rate(
                            (string)d["IpAddress"],
                            Convert.ToUInt16(d["Count"]),
                            Convert.ToDateTime(d["Ttl"]).ToUniversalTime()
                        )
                );
            }

            return rates;
        }
    }
}
