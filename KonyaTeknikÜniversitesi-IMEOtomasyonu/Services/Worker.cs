using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace IMEAutomationDBOperations.Services
{
    public class Worker : BackgroundService
    {
        private readonly DatabaseService _databaseService;

        public Worker(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _databaseService.CreateDatabase();
            _databaseService.CreateTables();
            _databaseService.GetUsersData();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
