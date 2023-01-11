
namespace SmartActorCore
{
    public interface IActorContext : IActorRef
    {
        IActorId GetSelf();

        IActorRef GetParentRef();

        void Tell(IActorId target, IActorMsg msg);

        void Stop(IActorId target);

        IActorRef GetOrCreateChildActor(IActorId actorId, Func<string> dispatcher, Func<IActorCreator> creator);

        void BroadcastToChildren(IActorMsg msg);

        void BroadcastToChildrenByType(IActorMsg msg, EntityType entityType);

        void BroadcastToChildren(IActorMsg msg, Predicate<IActorId> childFilter);

        List<IActorId> FilterChildren(Predicate<IActorId> childFilter);

    }
}
