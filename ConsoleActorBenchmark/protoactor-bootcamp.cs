using ConsoleActorBenchmark.Actors;
using ConsoleActorBenchmark.Messages;
using ConsoleActorBenchmark.Utils;
using Proto;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using static Proto.Remote.GrpcNet.GrpcNetRemoteConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto.Router;
using Proto.Persistence.Sqlite;
using Microsoft.Data.Sqlite;
using Messages;

namespace ConsoleActorBenchmark
{
    public class protoactor_bootcamp
    {

        public static async Task ExecutePersistenceActors()
        {
            var system = new ActorSystem();
            var context = new RootContext(system);
            var provider = new SqliteProvider(new SqliteConnectionStringBuilder { DataSource = "states.db" });

            var props = Props.FromProducer(() => new Calculator(provider));

            var pid1 = context.Spawn(props);

            system.Root.Send(pid1, new AddCommand { Value = 100 });
            system.Root.Send(pid1, new SubtractCommand { Value = 50 });

            Console.ReadKey();
            system.Root.Send(pid1, new PrintResultCommand());


            system.Root.Poison(pid1);


            var pid2 = context.Spawn(props);

            Console.ReadKey();
            system.Root.Send(pid2, new PrintResultCommand());
            Console.ReadLine();

        }


        public static async Task ExecuteRemoteServer()
        {
            var clients = new HashSet<PID>();

            var system = new ActorSystem();
            var props = Props.FromFunc(ctx =>
            {
                switch(ctx.Message)
                {
                    case chat.messages.Connect connect:
                      //  Console.WriteLine($"Client {connect.Sender} connected");
                     //  clients.Add(connect.Sender);
                       break;
                }
                return Task.CompletedTask;
            });
            system.Root.SpawnNamed(props, "chatserver");


        }





        public static async Task ExecuteEventStream()
        {
            var system = new ActorSystem();
            var props = Props.FromProducer(() => new Echo());
            var pid = system.Root.Spawn(props);

            system.EventStream.Subscribe<DeadLetterEvent>(msg => Console.WriteLine($"Sender: {msg.Sender}, Pid: {msg.Pid}, Message: {msg.Message}"));

            system.Root.Send(pid, "bla");
            await system.Root.PoisonAsync(pid);
            system.Root.Send(pid, "bla");

            Console.ReadLine();
        }


        private static readonly Props MyActorProps = Props.FromProducer(() => new MyActor());
        public async static Task ExecuteRouterPool()
        {
      

        var system = new ActorSystem();
        var context = new RootContext(system);
        var props = context.NewBroadcastPool(MyActorProps, 5);
        var pid = context.Spawn(props);
            for (var i = 0; i< 10; i++)
            {
                context.Send(pid, new Message ( $"{i % 4}" ));
            }


            Console.ReadKey();
        }

        public async static Task ExecuteRouterGroup()
        {


            var system = new ActorSystem();
            var context = new RootContext(system);
            var props = context.NewBroadcastGroup(
                context.Spawn(MyActorProps),
                context.Spawn(MyActorProps),
                context.Spawn(MyActorProps),
                context.Spawn(MyActorProps)
            );

            //var pid = context.Spawn(props);

            for (var i = 0; i < 10; i++)
            {
                var pid = context.Spawn(props);
                Console.WriteLine($"at {i} spwan: {pid}");
                context.Send(pid, new Message($"{i}"));
            }


            Console.ReadKey();
        }

        public async static Task ExecuteHierarchyAsync()
        {
                var system = new ActorSystem();
                Console.WriteLine("Actor system created");

                var props = Props.FromProducer(() => new PlaybackActor());
                var playbackPid = system.Root.Spawn(props);

                var actorPidMessage = await system.Root.RequestAsync<ResponseActorPidMessage>(playbackPid, new RequestActorPidMessage());
                var userCoordinatorActorPid = actorPidMessage.Pid;

                do
                {
                    ShortPause();

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    ColorConsole.WriteLineCyan("enter a command and hit enter");

                    var command = Console.ReadLine();

                    if (command != null)
                    {
                        if (command.StartsWith("play"))
                        {
                            var userId = int.Parse(command.Split(',')[1]);
                            var movieTitle = command.Split(',')[2];

                            system.Root.Send(userCoordinatorActorPid, new PlayMovieMessage(movieTitle, userId));
                        }
                        else if (command.StartsWith("stop"))
                        {
                            var userId = int.Parse(command.Split(',')[1]);

                            system.Root.Send(userCoordinatorActorPid, new StopMovieMessage(userId));
                        }
                        else if (command == "exit")
                        {
                            Terminate();
                        }
                    }
                } while (true);

                static void ShortPause()
                {
                    Thread.Sleep(250);
                }

                static void Terminate()
                {
                    Console.WriteLine("Actor system shutdown");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            
        }

        //public static void ExecuteBehavior()
        //{
        //    var system = new ActorSystem();
        //    Console.WriteLine("Actor system created");

        //    var props = Props.FromProducer(() => new UserActor());
        //    var pid = system.Root.Spawn(props);


        //    Console.ReadKey();
        //    Console.WriteLine("Sending PlayMovieMessage (The Movie)");
        //    system.Root.Send(pid, new PlayMovieMessage("The Movie", 44));
        //    Console.ReadKey();
        //    Console.WriteLine("Sending another PlayMovieMessage (The Movie 2)");
        //    system.Root.Send(pid, new PlayMovieMessage("The Movie 2", 54));

        //    Console.ReadKey();
        //    Console.WriteLine("Sending a StopMovieMessage");
        //    system.Root.Send(pid, new StopMovieMessage("The Movie 2", 54));

        //    Console.ReadKey();
        //    Console.WriteLine("Sending another StopMovieMessage");
        //    system.Root.Send(pid, new StopMovieMessage("The Movie 2", 54));

        //    Console.ReadLine();
        //}

        public static void ExecutePoisonPill()
        {
            var system = new ActorSystem();
            Console.WriteLine("Actor system created");

            var props = Props.FromProducer(() => new PlaybackActor());
            var pid = system.Root.Spawn(props);

            system.Root.Send(pid, new PlayMovieMessage("The Movie", 44));


            //system.Root.Stop(pid);
            system.Root.Poison(pid);

            Console.ReadLine();
        }

        public static void Execute()
        {
            var system = new ActorSystem();
            Console.WriteLine("Actor system created");



            var props = Props.FromProducer(() => new PlaybackActor()).WithChildSupervisorStrategy(new OneForOneStrategy(Decider.Decide, 1, null));
            var pid = system.Root.Spawn(props);

            system.Root.Send(pid, new PlayMovieMessage("The Movie", 44));
            system.Root.Send(pid, new PlayMovieMessage("The Movie 2", 54));
            system.Root.Send(pid, new PlayMovieMessage("The Movie 3", 64));
            system.Root.Send(pid, new PlayMovieMessage("The Movie 4", 74));

            Thread.Sleep(50);
            Console.WriteLine("press any key to restart actor");
            Console.ReadLine();

            system.Root.Send(pid, new Recoverable());

            Console.WriteLine("press any key to stop actor");
            Console.ReadLine();
            system.Root.Stop(pid);

            Console.ReadLine();
        }


        private class Decider
        {
            public static SupervisorDirective Decide(PID pid, Exception reason)
            {
                return SupervisorDirective.Restart;
            }
        }
    }
}
