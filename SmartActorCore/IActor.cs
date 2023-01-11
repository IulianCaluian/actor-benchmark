using SmartActorCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public interface IActor
    {
        IActorRef? ActorRef { get; }

        bool Process(IActorMsg msg)
        {
            return false;
        }

        void Init(IActorContext context);

        void Destroy()
        {

        }

        InitFailureStrategy OnInitFailure(int attempt, Exception ex)
        {
            return InitFailureStrategy.RetryWithDelay(5000L * attempt);
        }

        ProcessFailureStrategy OnProcessFailure(Exception ex)
        {
            if (ex is SystemException) {
                return ProcessFailureStrategy.Stop();
            } 
            else
            {
                return ProcessFailureStrategy.Resume();
            }
        }
    }
}
