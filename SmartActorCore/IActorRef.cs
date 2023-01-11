using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public interface IActorRef
    {
        IActorId ActorId { get; }

        void Tell(IActorMsg actorMsg);

        void TellWithHighPriority(IActorMsg actorMsg);
    }
}
