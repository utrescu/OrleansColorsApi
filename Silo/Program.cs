using System;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Orleans.Configuration;

namespace Silo
{
    class Program
    {
        // private const string Invariant = "System.Data.SqlClient";
        private const string Invariant = "MySql.Data.MySqlClient";
        // private const string ConnectionString = "Data Source=localhost;Initial Catalog=colors;Integrated Security=False;User ID=colors;Password=colors";
        private const string ConnectionString = "server=127.0.0.1;uid=colors;pwd=colors;database=colors; CharSet=utf8; SslMode = none";

        static ISiloHost silo;
        static bool siloStopping = false;
        static readonly object syncLock = new object();
        static readonly ManualResetEvent _siloStopped = new ManualResetEvent(false);

        static async Task Main(string[] args)
        {
            SetupApplicationShutdown();

            silo = CreateSilo();

            silo.StartAsync().Wait();

            _siloStopped.WaitOne();
        }

        static ISiloHost CreateSilo()
        {
            return new SiloHostBuilder()
                    .UseLocalhostClustering(serviceId: "colors-api")
                    // Evitar que acabi bruscament amb Ctrl+C
                    .Configure<ProcessExitHandlingOptions>(options => options.FastKillOnProcessExit = false)
                    .AddAdoNetGrainStorage("ColorsStorage", options =>
                    {
                        options.Invariant = Invariant;
                        options.ConnectionString = ConnectionString;
                        options.UseJsonFormat = true;
                    })
                    .ConfigureLogging(logging => logging.AddConsole())
                    .UseDashboard()
                    .Build();
        }

        // Molt del codi de l'aturada no abrupta està extret de la documentació
        static async Task StopSilo()
        {
            await silo.StopAsync();
            _siloStopped.Set();
        }

        static void SetupApplicationShutdown()
        {
            /// El Silo s'aturarà quan es premi Ctrl+C
            Console.CancelKeyPress += (s, a) =>
            {
                a.Cancel = true;
                /// Evitar que es pugui fer servir Ctrl+C diverses vegades
                lock (syncLock)
                {
                    if (!siloStopping)
                    {
                        siloStopping = true;
                        Task.Run(StopSilo).Ignore();
                    }
                }
            };
        }
    }
}
