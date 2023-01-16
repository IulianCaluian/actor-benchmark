using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Messages
{
    public class IncrementPlayCountMessage
    {
        public string MovieTitle { get; }

        public IncrementPlayCountMessage(string movieTitle)
        {
            MovieTitle = movieTitle;
        }
    }
}
