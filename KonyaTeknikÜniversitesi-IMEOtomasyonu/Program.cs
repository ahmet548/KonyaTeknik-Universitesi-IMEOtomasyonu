using IMEAutomationDBOperations.Services;
using IMEAutomationDBOperations.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IMEAutomationDBOperations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                string connectionString = @"Data Source=DESKTOP-JAU4GNF\MSSQLSERVER01; Initial Catalog=InternshipDB; Integrated Security=True; TrustServerCertificate=True;";

                services.AddSingleton<IRepository>(new SqlRepository(connectionString));
                services.AddSingleton<DatabaseService>();

                services.AddHostedService<Worker>();
            });
    }
}
