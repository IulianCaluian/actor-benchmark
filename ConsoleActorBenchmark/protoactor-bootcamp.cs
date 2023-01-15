using ConsoleActorBenchmark.Actors;
using ConsoleActorBenchmark.Messages;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark
{
    public class protoactor_bootcamp
    {

        public static void ExecuteBehavior()
        {
            var system = new ActorSystem();
            Console.WriteLine("Actor system created");

            var props = Props.FromProducer(() => new UserActor());
            var pid = system.Root.Spawn(props);


            Console.ReadKey();
            Console.WriteLine("Sending PlayMovieMessage (The Movie)");
            system.Root.Send(pid, new PlayMovieMessage("The Movie", 44));
            Console.ReadKey();
            Console.WriteLine("Sending another PlayMovieMessage (The Movie 2)");
            system.Root.Send(pid, new PlayMovieMessage("The Movie 2", 54));

            Console.ReadKey();
            Console.WriteLine("Sending a StopMovieMessage");
            system.Root.Send(pid, new StopMovieMessage());

            Console.ReadKey();
            Console.WriteLine("Sending another StopMovieMessage");
            system.Root.Send(pid, new StopMovieMessage());

            Console.ReadLine();
        }

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
