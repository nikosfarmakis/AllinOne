using AllinOne.Models.Configuration;
using Microsoft.Extensions.Options;

namespace AllinOne.Services.Implementations
{
    public class ValidationStartupCheck : IHostedService
    {
        private readonly IOptionsMonitor<AccessSection> _monitor;

        public ValidationStartupCheck(IOptionsMonitor<AccessSection> monitor)
        {
            _monitor = monitor;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var _ = _monitor.CurrentValue; // triggers ValidationUsersSectionValidator
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
