using System;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using System.Threading.Tasks;

namespace Silo
{
    class Program
    {
        // private const string Invariant = "System.Data.SqlClient";
        private const string Invariant = "MySql.Data.MySqlClient";
        // private const string ConnectionString = "Data Source=localhost;Initial Catalog=colors;Integrated Security=False;User ID=colors;Password=colors";
        private const string ConnectionString = "server=127.0.0.1;uid=colors;pwd=colors;database=colors; CharSet=utf8; SslMode = none";
        static async Task Main(string[] args)
        {
            var siloBuilder = new SiloHostBuilder()
                .UseLocalhostClustering(serviceId: "colors-api")
                .AddAdoNetGrainStorage("ColorsStorage", options =>
                    {
                        options.Invariant = Invariant;
                        options.ConnectionString = ConnectionString;
                        options.UseJsonFormat = true;
                    })
                .ConfigureLogging(logging => logging.AddConsole())
                .UseDashboard();


            using (var host = siloBuilder.Build())
            {
                await host.StartAsync();
                Console.ReadLine();
            }
        }
    }
}
