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

        public HashGeneratorGrain()
        {
            this.hashAlgorithm = MD5.Create();
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

        public async Task<string> GenerateHashAsync(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = hashAlgorithm.ComputeHash(inputBytes);
            var hashBase64Str = Convert.ToBase64String(hashBytes);

            return hashBase64Str;
        }
    }
}