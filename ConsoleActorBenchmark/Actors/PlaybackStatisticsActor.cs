using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Actors
{
    internal class PlaybackStatisticsActor
    {
        private PID _moviePlayCounterActorRef;

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started msg:
                    var props = Props.FromProducer(() => new MoviePlayCounterActor());
                    _moviePlayCounterActorRef = context.Spawn(props);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
