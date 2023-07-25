using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MuaVatTu.Business;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MuaVatTu.API.v1
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public MyBackgroundService(IServiceProvider serviceProvider, ILogger<MyBackgroundService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation("From MyBackgroundService: ExcuteAsync {dateTime}", DateTime.Now);
                    var scopeService = scope.ServiceProvider.GetService<IBoPhanHandler>();
                    scopeService.GetBoPhan();
                    await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                }
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("From MyBackgroundService: StopAsync {dateTime}", DateTime.Now);
            return base.StopAsync(cancellationToken);
        }
    }
}