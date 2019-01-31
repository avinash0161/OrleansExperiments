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
    public class TempGrain : Grain, ITempGrain
    {
        private bool call;

        private void ToggleCall()
        {
            call = call?false:true;
        }

        public async Task TempCall()
        {
            Console.WriteLine("Temp Grain received a call");
            await Task.Delay(20000);
            return;
        }

        public async Task CallA()
        {
            Console.WriteLine("Call A came to TempGrain");
            await Task.Delay(20000);
        }

        public Task CallB()
        {
            Console.WriteLine("Call B came to TempGrain");
            return Task.CompletedTask;
        }
    }
}