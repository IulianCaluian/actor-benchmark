using Microsoft.VisualBasic;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark
{
    internal class TestingContext
    {
        public static async void Execute()
        {

            ActorSystem sys = new ActorSystem();
            var pingProps = Props.FromProducer(() => new PingActor());
            var echoProps = Props.FromProducer(() => new EchoActor());
            
            var ping = sys.Root.Spawn(pingProps);
            var echo = sys.Root.Spawn(echoProps);



            sys.Root.Send(ping, new PidAndText() { Pid = echo, Text = "text2345" });
       


            Console.ReadKey();
        }


        public class PidAndText
        {
            public PID? Pid { get; set; }  
            public string? Text { get; set; }
        }


        public class PingActor : IActor
        {
            public async Task ReceiveAsync(IContext context)
            {
                if (context.Message is PidAndText)
                {
                    var msg = (PidAndText)context.Message;

                    Console.WriteLine($"messag to pid ({msg.Pid}), with pid {context.Self}: {msg.Text}");

                    var dataTask  = context.RequestAsync<string>(msg.Pid, new PidAndText() { Pid = context.Self, Text = "bla" });

                    context.ReenterAfter(dataTask, data => {
                        Console.WriteLine("Received back" + data);
                    });

                
                } else if (context.Message is string)
                {
                    Console.WriteLine("Received a string." + context.Message);
                }
            }
        }

        public class EchoActor : IActor
        {
            public Task ReceiveAsync(IContext context)
            {
                if (context.Message is PidAndText)
                {
                    var msg = (PidAndText)context.Message;

                    Console.WriteLine($"message-to-echo: {msg.Text}");

                    context.Send(msg.Pid, "msg-back");

                   
                }

                return Task.CompletedTask;

            }
        }
    }
}
