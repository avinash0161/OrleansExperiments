using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    public class OperatorC : Grain, IOperatorC
    {
        public async Task Process(int i)
        {
            await Task.Delay(2000);
            Console.WriteLine("C: processed "+i.ToString());
            
        }

        public Task ReceiveTuple(int i)
        {
            Console.WriteLine("C: received "+i.ToString());
            GrainFactory.GetGrain<IOperatorC>(this.GetPrimaryKeyLong()).ToProcess(i);
            return Task.CompletedTask;
        }

        public Task ToProcess(int i)
        {
            GrainFactory.GetGrain<IOperatorC>(this.GetPrimaryKeyLong()).Process(i).ContinueWith((t)=>
            {
                if(t.IsFaulted)
                {
                    GrainFactory.GetGrain<IOperatorC>(this.GetPrimaryKeyLong()).ToProcess(i);
                }
            });
            return Task.CompletedTask;
        }


    }
}