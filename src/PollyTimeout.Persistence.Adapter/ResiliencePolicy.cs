using System;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace PollyTimeout.Persistence.Adapter
{
    public class ResiliencePolicy
    {
        public ResiliencePolicy()
        {
            Policy = Init();
        }

        public Policy Policy { get; }

        protected virtual Policy Init()
        {
            var timeoutPolicy = Policy.TimeoutAsync(
                TimeSpan.FromMilliseconds(100),
                TimeoutStrategy.Pessimistic,
                OnTimeoutAsync);

            return timeoutPolicy;
        }

        protected virtual Task OnTimeoutAsync(Context context, TimeSpan timeSpan, Task task)
        {
            return Task.FromResult(0);
        }
    }
}