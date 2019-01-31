using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    public class OperatorA : Grain, IOperatorA
    {
        public async Task Process(int i)
        {
            await Task.Delay(3000);
            Console.WriteLine("A: processed "+i.ToString());
            GrainFactory.GetGrain<IOperatorB>(this.GetPrimaryKeyLong()).ReceiveTuple(i).ContinueWith((t)=>
            {
                if(t.IsFaulted)
                {
                    GrainFactory.GetGrain<IOperatorB>(this.GetPrimaryKeyLong()).ReceiveTuple(i);
                }
            });
        }

        public Task ReceiveTuple(int i)
        {
            Console.WriteLine("A: received "+i.ToString());
            GrainFactory.GetGrain<IOperatorA>(this.GetPrimaryKeyLong()).ToProcess(i);
            return Task.CompletedTask;
        }

        public Task ToProcess(int i)
        {
            GrainFactory.GetGrain<IOperatorA>(this.GetPrimaryKeyLong()).Process(i).ContinueWith((t)=>
            {
                if(t.IsFaulted)
                {
                    // check for timeout exception
                    GrainFactory.GetGrain<IOperatorA>(this.GetPrimaryKeyLong()).ToProcess(i);
                }
            });
            return Task.CompletedTask;
        }


    }
}