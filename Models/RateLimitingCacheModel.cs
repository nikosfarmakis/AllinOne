
namespace AllinOne.Models
{
    public class RateLimitingCacheModel
    {
        public int MaxRequests { get; init; }
        public List<DateTime> CounterRequests { get; private set; }
        public int ExpirationInSeconds {  get; init; }
        public RateLimitingCacheModel(int maxRequests, int expirationInSeconds)
        {
            this.ExpirationInSeconds = expirationInSeconds;
            this.MaxRequests = maxRequests;
            CounterRequests = new List<DateTime>();
            CounterRequests.Add(DateTime.UtcNow);
        }

        public virtual bool CheckAvailableRequestsAndIncrementCounter()
        {
            CounterRequests.RemoveAll(ts => (DateTime.UtcNow - ts).TotalSeconds > ExpirationInSeconds);


            if (CounterRequests.Count < MaxRequests)
            {
                CounterRequests.Add(DateTime.UtcNow);
                return true;
            }
            return false;
        }

    }
}
