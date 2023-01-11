using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartActorCore
{
    public class ProcessFailureStrategy
    {
        private bool _stop;

        private ProcessFailureStrategy(bool stop)
        {
            _stop = stop;
        }

        public static ProcessFailureStrategy Stop()
        {
            return new ProcessFailureStrategy(true);
        }

        public static ProcessFailureStrategy Resume()
        {
            return new ProcessFailureStrategy(false);
        }

        public override string ToString()
        {
            return $"ProcessFailureStrategy(stop={_stop})";
        }

    }
}
