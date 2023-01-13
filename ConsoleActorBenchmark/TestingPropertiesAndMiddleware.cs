using Proto.Mailbox;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace ConsoleActorBenchmark
{
    internal class TestingPropertiesAndMiddleware
    {

        public static void Execute()
        {
            var system = new ActorSystem();

            Producer actorFactory = () => new PrintActor();

            var pid1 = system.Root.Spawn(Props.FromProducer(actorFactory));
            var pid2 = system.Root.Spawn(Props.FromProducer(actorFactory));
            var pid3 = system.Root.Spawn(Props.FromProducer(actorFactory));

            Console.WriteLine($"PID is {pid1.Id}");
            Console.WriteLine($"PID is {pid2.Id}");
            Console.WriteLine($"PID is {pid3.Id}");

            system.Root.Send(pid1, new ObjectText() { Text = "test-string" });
            system.Root.Send(pid1, new ObjectText() { Text = "test-string" });

            Console.ReadLine();

            var props = new Props()
        // the producer is a delegate that returns a new instance of an IActor
        .WithProducer(() => new ForwordingActor())
        // the default dispatcher uses the thread pool and limits throughput to 300 messages per mailbox run
        .WithDispatcher(new ThreadPoolDispatcher { Throughput = 300 })
        // the default mailbox uses unbounded queues
        .WithMailbox(() => UnboundedMailbox.Create())
        // the default strategy restarts child actors a maximum of 10 times within a 10 second window
        .WithChildSupervisorStrategy(new OneForOneStrategy((who, reason) => SupervisorDirective.Restart, 10, TimeSpan.FromSeconds(10)))
        // middlewares can be chained to intercept incoming and outgoing messages
        // receive middlewares are invoked before the actor receives the message
        // sender middlewares are invoked before the message is sent to the target PID
        .WithReceiverMiddleware(
            next => async (c, envelope) =>
            {
                Console.WriteLine($"r middleware 1 enter {envelope.Message.GetType()} : {envelope.Message}");
                await next(c, envelope);
                Console.WriteLine($"r middleware 1 exit  {envelope.Message.GetType()} : {envelope.Message}");
            },
            next => async (c, envelope) =>
            {
                Console.WriteLine($"r middleware 2 enter {envelope.Message.GetType()} : {envelope.Message}");
                await next(c, envelope);
                Console.WriteLine($"r middleware 2 exit  {envelope.Message.GetType()} : {envelope.Message}");
            })
        .WithSenderMiddleware(
            next => async (c, target, envelope) =>
            {
                Console.WriteLine($"s middleware 1 enter {c.Message.GetType()} : {c.Message}");

                await next(c, target, envelope);
                Console.WriteLine($"s middleware 1 exit {c.Message.GetType()} : {c.Message}");
            },
            next => async (c, target, envelope) =>
            {
                Console.WriteLine($"s middleware 2 enter {c.Message.GetType()} : {c.Message}");
                await next(c, target, envelope);
                Console.WriteLine($"s middleware 2 exit {c.Message.GetType()} : {c.Message}");
            })
        //the default spawner constructs the Actor, Context and Process
        .WithSpawner(Props.DefaultSpawner);

            Console.ReadKey();
            var pid4 = system.Root.Spawn(props);
            Console.ReadKey();
            var pid5 = system.Root.Spawn(props);

            Console.WriteLine("Ready to send?");
            Console.ReadKey();
            Console.WriteLine("--------------------");

            system.Root.Send(pid4,

                new DoubleForwording()
                {
                    ForwordingPID = pid5,
                    NextForwording = new ObjectForwordingText() { ForwordingPID = pid3, Text = "txt-to-fwd" }
                });


            Console.ReadKey();
        }




        public class ObjectText
        {
            public string? Text { get; set; }

            public override string ToString()
            {
                return Text ?? string.Empty;
            }
        }

        public class DoubleForwording
        {

            public PID? ForwordingPID { get; set; }
            public ObjectForwordingText NextForwording { get; set; }

            public override string ToString()
            {
                return $"d-fwd:{NextForwording.Text}";
            }
        }

        public class ObjectForwordingText
        {
            public string? Text { get; set; }
            public PID? ForwordingPID { get; set; }

            public override string ToString()
            {
                return $"fwd:{Text}";
            }
        }

        private class PrintActor : IActor
        {
            public Task ReceiveAsync(IContext context)
            {
                var msg = context.Message;

                if (msg is ObjectText ot)
                {
                    Console.WriteLine($"text: {ot.Text}");
                }
                return Task.CompletedTask;
            }
        }

        private class ForwordingActor : IActor
        {
            public Task ReceiveAsync(IContext context)
            {
                var msg = context.Message;

                if (msg is ObjectText ot)
                {
                    Console.WriteLine($"no-forwarding: {ot.Text}");
                }
                else if (msg is ObjectForwordingText oft)
                {
                    Console.WriteLine($"simple-forwording-text: {oft.Text}");
                    context.Send(oft.ForwordingPID, new ObjectText() { Text = oft.Text });
                }
                else if (msg is DoubleForwording dfm)
                {
                    Console.WriteLine($"double-forwording-text: {dfm.NextForwording.Text}");
                    context.Send(dfm.ForwordingPID, dfm.NextForwording);
                }

                return Task.CompletedTask;
            }
        }

    }


}
