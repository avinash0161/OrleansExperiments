using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    public class OperatorB : Grain, IOperatorB
    {
        public async Task Process(int i)
        {
            await Task.Delay(5000);
            Console.WriteLine("B: processed "+i.ToString());
            GrainFactory.GetGrain<IOperatorC>(this.GetPrimaryKeyLong()).ReceiveTuple(i).ContinueWith((t)=>
            {
                if(t.IsFaulted)
                {
                    GrainFactory.GetGrain<IOperatorC>(this.GetPrimaryKeyLong()).ReceiveTuple(i);
                }
            });
        }

        public Task ReceiveTuple(int i)
        {
            Console.WriteLine("B: received "+i.ToString());
            GrainFactory.GetGrain<IOperatorB>(this.GetPrimaryKeyLong()).ToProcess(i);
            return Task.CompletedTask;
        }

        public Task ToProcess(int i)
        {
            GrainFactory.GetGrain<IOperatorB>(this.GetPrimaryKeyLong()).Process(i).ContinueWith((t)=>
            {
                if(t.IsFaulted)
                {
                    GrainFactory.GetGrain<IOperatorB>(this.GetPrimaryKeyLong()).ToProcess(i);
                }
            });
            return Task.CompletedTask;
        }


    }
}