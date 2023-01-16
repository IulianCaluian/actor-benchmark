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
    public class PlaybackActor : IActor
    {
        private PID _moviePlayCounterActorRef;
        private PID _userCoordinatorActorRef;

        public PlaybackActor() => Console.WriteLine("Creating a PlaybackActor");
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started msg:
                    ProcessStartedMessage(context, msg);
                    break;

                case PlayMovieMessage msg:
                    ProcessPlayMovieMessage(context, msg);      
                    break;

                case StopMovieMessage msg:
                    ProcessStopMovieMessage(context, msg);
                    break;

                case Recoverable msg:
                    ProcessRecoverableMessage(context, msg);
                    break;

                case Stopping msg:
                    ProcessStoppingMessage(msg);
                    break;


                case Stopped msg:
                    Console.WriteLine("actor is Stopped");
                    break;

                case IncrementPlayCountMessage:
                    // pid_playbackStatistics

                    Console.WriteLine("Try to send statistics");
                    break;

                case RequestActorPidMessage msg:
                    ProcessRequestActorPidMessage(context, msg);
                    break;
            }
            return Task.CompletedTask;
        }

  

        private void ProcessStopMovieMessage(IContext context, StopMovieMessage msg)
        {
            ColorConsole.WriteLineYellow($"StopMovieMessage for user {msg.UserId}");

            context.Send(_userCoordinatorActorRef, msg);
        }

        private void ProcessPlayMovieMessage(IContext context,  PlayMovieMessage msg)
        {
            ColorConsole.WriteLineYellow($"PlayMovieMessage {msg.MovieTitle}  for user {msg.UserId}");
            context.Send(_userCoordinatorActorRef, msg);
        }

        private void ProcessStartedMessage(IContext context, Started msg)
        {
            ColorConsole.WriteLineGreen("PlaybackActor Started");

            var moviePlayCounterActorProps = Props.FromProducer(() => new MoviePlayCounterActor());
            _moviePlayCounterActorRef = context.Spawn(moviePlayCounterActorProps);

            _userCoordinatorActorRef = context.Spawn(Props.FromProducer(() => new UserCoordinatorActor(_moviePlayCounterActorRef)));
        }

        private void ProcessStoppingMessage(Stopping msg)
        {
            ColorConsole.WriteLineGreen("PlaybackActor Stopping");
        }

        private void ProcessRecoverableMessage(IContext context, Recoverable msg)
        {


            PID child;

            if (context.Children == null || context.Children.Count == 0)
            {
                var props = Props.FromProducer(() => new ChildActor());
                child = context.Spawn(props);
            }
            else
            {
                child = context.Children.First();
            }

            context.Forward(child);
        }


        private void ProcessRequestActorPidMessage(IContext context, RequestActorPidMessage msg)
        {
            context.Respond(new ResponseActorPidMessage(_userCoordinatorActorRef));
        }
    }
}
