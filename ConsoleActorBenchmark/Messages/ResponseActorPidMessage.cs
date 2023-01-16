using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Messages
{
    public class ResponseActorPidMessage
    {
        public PID Pid { get; }

        public ResponseActorPidMessage(PID pid)
        {
            Pid = pid;
        }
    }
}
