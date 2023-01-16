using ConsoleActorBenchmark.Messages;
using ConsoleActorBenchmark.Utils;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Actors
{
    internal class UserCoordinatorActor : IActor
    {
        private readonly PID _moviePlayCounterActorRef;
        private readonly Dictionary<int, PID> _users = new Dictionary<int, PID>();

        public UserCoordinatorActor(PID moviePlayCounterActorRef)
        {
            _moviePlayCounterActorRef = moviePlayCounterActorRef;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case IncrementPlayCountMessage:
                    context.Send(context.Parent, context.Message);
                    break;

                case PlayMovieMessage msg:
                    ProcessPlayMovieMessage(context, msg);
                    break;

                case StopMovieMessage msg:
                    ProcessStopMovieMessage(context, msg);
                    break;
            }
            ColorConsole.WriteLineCyan("UserActor has now become Stopped");

            return Task.CompletedTask;
        }

        private void ProcessPlayMovieMessage(IContext context, PlayMovieMessage msg)
        {
            CreateChildUserIfNotExists(context, msg.UserId);
            var childActorRef = _users[msg.UserId];
            context.Send(childActorRef, msg);
        }

        private void ProcessStopMovieMessage(IContext context, StopMovieMessage msg)
        {
            CreateChildUserIfNotExists(context, msg.UserId);
            var childActorRef = _users[msg.UserId];
            context.Send(childActorRef, msg);
        }

        private void CreateChildUserIfNotExists(IContext context, int userId)
        {
            if (!_users.ContainsKey(userId))
            {
                var props = Props.FromProducer(() => new UserActor(userId, _moviePlayCounterActorRef));
                var pid = context.SpawnNamed(props, $"User{userId}");
                _users.Add(userId, pid);
                ColorConsole.WriteLineCyan($"UserCoordinatorActor created new child UserActor for {userId} (Total Users: {_users.Count})");
            }
        }
    }
}
