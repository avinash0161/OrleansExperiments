using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    public interface ITempGrain : IGrainWithIntegerKey
    {
        Task TempCall();
        // Task<string> GenerateHashAsync(string input);
    }
}