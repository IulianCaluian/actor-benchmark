using ConsoleActorBenchmark.Messages;
using Microsoft.VisualBasic;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Actors
{
    public class MyActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch(context.Message)
            {
                case Message msg:
                    Console.WriteLine(context.Self + " " + msg.Text);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
