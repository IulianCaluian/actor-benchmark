using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public abstract class AbstractActor : IActor
    {
        protected IActorContext? context;

        public IActorRef? ActorRef => context;

        public void Init(IActorContext context)
        {
            this.context = context;
        }
    }
}
