using ConsoleActorBenchmark.Messages;
using ConsoleActorBenchmark.Utils;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleActorBenchmark.Actors
{
    internal class MoviePlayCounterActor : IActor
    {
        private Dictionary<string, int> _moviePlayCounts = new Dictionary<string, int>();
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case IncrementPlayCountMessage msg:
                    ProcessIncrementPlayCountMessage(msg);
                    break;
            }
            return Task.CompletedTask;
        }

        private void ProcessIncrementPlayCountMessage(IncrementPlayCountMessage message)
        {
            if (!_moviePlayCounts.ContainsKey(message.MovieTitle))
            {
                _moviePlayCounts.Add(message.MovieTitle, 0);
            }
            _moviePlayCounts[message.MovieTitle]++;

            ColorConsole.WriteLineYellow($"MoviePlayerCounterActor '{message.MovieTitle}' has been watched {_moviePlayCounts[message.MovieTitle]} times");
        }
    }
}
