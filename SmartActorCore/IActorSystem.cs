using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public interface IActorSystem
    {

        // void createTask(String taskId, Task task);

        // void destroyTask(String taskId);

        IActorRef GetActor(IActorId actorId);

        IActorRef CreateRootActor(String dispatcherId, IActorCreator creator);

        IActorRef CreateChildActor(String dispatcherId, IActorCreator creator, IActorId parent);

        void Tell(IActorId target, IActorMsg actorMsg);

        void TellWithHighPriority(IActorId target, IActorMsg actorMsg);

        void Stop(IActorRef actorRef);

        void Stop(IActorId actorId);

        void Stop();

        void BroadcastToChildren(IActorId parent, IActorMsg msg);

        void BroadcastToChildren(IActorId parent, Predicate<IActorId> childFilter, IActorMsg msg);

        List<IActorId> FilterChildren(IActorId parent, Predicate<IActorId> childFilter);

    }
}
