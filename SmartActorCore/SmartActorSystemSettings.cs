using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public class SmartActorSystemSettings
    {
        public int ActorThroughput { get; set; }
        public int SchedulerPoolSize { get; set; }
        public int MaxActorInitAttempts { get; set; }

        public SmartActorSystemSettings(int actorThroughput, int schedulerPoolSize, int maxActorInitAttempts)
        {
            ActorThroughput = actorThroughput;
            SchedulerPoolSize = schedulerPoolSize;
            MaxActorInitAttempts = maxActorInitAttempts;
        }
    }
}
