using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Messages
{
    internal class StopMovieMessage
    {

        public int UserId { get; }

        public StopMovieMessage(int userId)
        {
            UserId = userId;
        }
    }
}
