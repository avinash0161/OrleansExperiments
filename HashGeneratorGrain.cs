using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    // [StatelessWorker]
    public class HashGeneratorGrain : Grain, IHashGeneratorGrain
    {
        private HashAlgorithm hashAlgorithm;

        private int counter;

        public HashGeneratorGrain()
        {
            this.hashAlgorithm = MD5.Create();
            this.counter = 0;
        }

        public async Task TempCall()
        {
            Console.WriteLine($"TempCall received a call at {DateTime.Now}");
            await Task.Delay(33000);
            Console.WriteLine($"TempCall completed at {DateTime.Now}");
            return;
        }

        public async Task CallToFellowGrain()
        {
            try
            {
                Console.WriteLine("Making call to a fellow grain");
                ITempGrain grain = this.GrainFactory.GetGrain<ITempGrain>(1);
                await grain.TempCall();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public async Task Call_A_ToTemp()
        {
            Console.WriteLine("Making call A to a fellow grain");
            ITempGrain grain = this.GrainFactory.GetGrain<ITempGrain>(1);

            // SendNext;
            grain.CallA().ContinueWith(async (t)=> 
            {
                if(t.IsFaulted)
                {
                    Console.WriteLine("Task Faulted");
                    Call_A_ToTemp();
                }
                else if (t.IsCompletedSuccessfully)
                {
                    Console.WriteLine("Task success");
                    Console.WriteLine("Call_A_ToTemp: Counter value now is: " + counter);
                    var selfGrain = this.GrainFactory.GetGrain<IHashGeneratorGrain>(0);
                    await selfGrain.IncCounter();
                    Console.WriteLine("Call_A_ToTemp: after increment: " + this.counter);
                }
            }
            );

        }

        public async Task IncCounter() {
            this.counter += 1;
        }

        public async Task Call_B_ToTemp()
        {
            // Console.WriteLine("Making call B to a fellow grain");
            // ITempGrain grain = this.GrainFactory.GetGrain<ITempGrain>(1);
            // await grain.CallB();

            Console.WriteLine("Call_B_ToTemp called");
            Console.WriteLine("Call_B_ToTemp: Counter value now is: " + counter);
            Console.WriteLine("Call_B_ToTemp: now sleep for 20s");
            await Task.Delay(20000);
            Console.WriteLine("Call_B_ToTemp: after sleep value: " + counter);
            this.counter += 1;
            Console.WriteLine("Call_B_ToTemp: after increment: " + this.counter);
        }

        public async Task<string> GenerateHashAsync(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = hashAlgorithm.ComputeHash(inputBytes);
            var hashBase64Str = Convert.ToBase64String(hashBytes);

            return hashBase64Str;
        }
    }
}