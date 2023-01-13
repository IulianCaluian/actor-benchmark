using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public class DefaultSmartActorSystem : IActorSystem
    {
        public DefaultSmartActorSystem(SmartActorSystemSettings settings)
        {
            throw new NotImplementedException();
        }

        public void BroadcastToChildren(IActorId parent, IActorMsg msg)
        {
            throw new NotImplementedException();
        }

        public void BroadcastToChildren(IActorId parent, Predicate<IActorId> childFilter, IActorMsg msg)
        {
            throw new NotImplementedException();
        }

        public IActorRef CreateChildActor(string dispatcherId, IActorCreator creator, IActorId parent)
        {
            throw new NotImplementedException();
        }

        public IActorRef CreateRootActor(string dispatcherId, IActorCreator creator)
        {
            throw new NotImplementedException();
        }

        public List<IActorId> FilterChildren(IActorId parent, Predicate<IActorId> childFilter)
        {
            throw new NotImplementedException();
        }

        public IActorRef GetActor(IActorId actorId)
        {
            throw new NotImplementedException();
        }

        public void Stop(IActorRef actorRef)
        {
            throw new NotImplementedException();
        }

        public void Stop(IActorId actorId)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Tell(IActorId target, IActorMsg actorMsg)
        {
            throw new NotImplementedException();
        }

        public void TellWithHighPriority(IActorId target, IActorMsg actorMsg)
        {
            throw new NotImplementedException();
        }
    }
}
