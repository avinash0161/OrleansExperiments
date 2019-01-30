using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orleans2StatelessWorkers
{
    public interface IHashGeneratorGrain : IGrainWithIntegerKey
    {
        Task CallToFellowGrain();
        Task TempCall();
        Task<string> GenerateHashAsync(string input);
        Task Call_A_ToTemp();
        Task Call_B_ToTemp();
    }
}
