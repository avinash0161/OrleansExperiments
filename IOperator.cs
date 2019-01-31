using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    public interface IOperator : IGrainWithIntegerKey
    {
        Task ReceiveTuple(int i);
        Task ToProcess(int i);
        Task Process(int i);
    }
}
