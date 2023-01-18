using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Actors
{
    public class Echo : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            return Task.CompletedTask;
        }
    }
}
