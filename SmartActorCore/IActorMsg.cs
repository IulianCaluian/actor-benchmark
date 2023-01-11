using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public interface IActorMsg
    {

        MsgType MsgType { get; }

        /**
         * Executed when the target SmartActor is stopped or destroyed.
         * For example, rule node failed to initialize or removed from rule chain.
         * Implementation should cleanup the resources.
         */
        void OnSmartActorStopped(SmartActorStopReason reason)
        {
        }

    }

    public enum SmartActorStopReason
    {

        INIT_FAILED, STOPPED

    }
}
