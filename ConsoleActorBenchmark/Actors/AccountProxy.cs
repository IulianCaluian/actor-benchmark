using ConsoleActorBenchmark.Messages;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Actors
{
    public class AccountProxy : IActor
    {
        private readonly PID _account;
        private readonly Func<PID, object> _createMessage;
        public AccountProxy(PID account, Func<PID, object> createMessage)
        {
            _account = account;
            _createMessage = createMessage;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch(context.Message)
            {
                case Started _:
                    context.Send(_account, _createMessage(context.Self));
                    context.SetReceiveTimeout(TimeSpan.FromMilliseconds(100));
                    break;
                case OK msg:
                    context.CancelReceiveTimeout();
                    context.Send(context.Parent, msg);
                    break;
                case Refused msg:
                    context.CancelReceiveTimeout();
                    context.Send(context.Parent, msg);
                    break;
                // These represent a failed remote call
                case InternalServerError _:
                case ReceiveTimeout _:
                case ServiceUnavailable _:
                    throw new Exception();
            }

            return Task.CompletedTask;
        }
    }
}
