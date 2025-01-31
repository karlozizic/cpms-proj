using OCPP.Core.Database;

namespace OCPP.Core.Management.Services;

using System.Linq;
using System.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class LoadBalancingService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Timer _timer;
    private readonly ILogger<LoadBalancingService> _logger;

    public LoadBalancingService(IServiceScopeFactory serviceScopeFactory, ILogger<LoadBalancingService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _timer = new Timer(5000);
        _timer.Elapsed += RunLoadBalancing;
        _timer.AutoReset = true;
    }

    public void Start() => _timer.Start();

    private void RunLoadBalancing(object sender, ElapsedEventArgs e)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OCPPCoreContext>();

            var locations = context.ChargeLocations.ToList();
            foreach (var location in locations)
            {
                _logger.LogInformation($"Balansiram lokaciju: {location.LocationId}");
            }
        }
    }
}
