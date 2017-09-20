using System;
using System.Threading.Tasks;
using Polly;
using PollyTimeout.Persistence.Adapter;

namespace PollyTimeout.Persistence.Adapters.Tests.Unit.Stubs
{
    public class ResiliencePolicyStub : ResiliencePolicy
    {
        public bool TimeoutTriggered { get; private set; }

        protected override async Task OnTimeoutAsync(Context context, TimeSpan timeSpan, Task task)
        {
            TimeoutTriggered = true;
            await base.OnTimeoutAsync(context, timeSpan, task);
        }
    }
}