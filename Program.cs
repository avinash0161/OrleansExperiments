using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    class Program
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static Random random;

        static string GenerateRandomString() // ref: https://stackoverflow.com/a/1344258/983064
        {
            var stringChars = new char[8];

            for (int i = 0; i < stringChars.Length; i++)
            {
                var randomCharIndex = random.Next(chars.Length);
                stringChars[i] = chars[randomCharIndex];
            }

            return new string(stringChars);
        }

        static async Task Main(string[] args)
        {
            random = new Random();

            // 1. set up silo

            var siloBuilder = new SiloHostBuilder()
                .UseLocalhostClustering()
                //.UseDashboard(options => { })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Orleans2StatelessWorkers";
                })
                .Configure<EndpointOptions>(options =>
                    options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureLogging(logging => logging.AddConsole()) // options.ResendOnTimeout = true; options.MaxResendCount = 60; 
                .Configure<SiloMessagingOptions>(options => {options.ResponseTimeout = new TimeSpan(0,0,32); });

            var host = siloBuilder.Build();
            await host.StartAsync();

            // 2. set up client

            var clientBuilder = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "Orleans2StatelessWorkers";
                })
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Critical).AddConsole())
                .Configure<ClientMessagingOptions>(options => {options.ResponseTimeout = new TimeSpan(0,0,130);});

            var client = clientBuilder.Build();
            await client.Connect();

            // 3. generate load

            var hashGenerator = client.GetGrain<IHashGeneratorGrain>(0);

            //while (true)
            //{
                // var randomString = GenerateRandomString();
                // var hash = await hashGenerator.GenerateHashAsync(randomString);
                // Console.WriteLine(hash);

                try{
                    Console.WriteLine("Client making a call");
                    //await hashGenerator.CallToFellowGrain();
                    // await hashGenerator.TempCall();
                    hashGenerator.Call_A_ToTemp();
                    await Task.Delay(1000);
                    hashGenerator.Call_B_ToTemp();
                    // hashGenerator.TempCall().ContinueWith((t)=>
                    //     {
                    //         if(t.IsFaulted)
                    //         {
                    //             Console.WriteLine("Got faulted");
                    //             hashGenerator.TempCall();
                    //         }
                    //         // else if(t.I)
                    //     }
                    // );
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION: " + ex.Message);
                }

            //}

            // 4. cleanup - never reached in this case
            Console.ReadKey();
            await client.Close();
            await host.StopAsync();
        }
    }
}