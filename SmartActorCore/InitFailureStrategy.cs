
namespace SmartActorCore
{
    public class InitFailureStrategy
    {
        private bool _stop;
        private long _retryDelay;

        private InitFailureStrategy(bool stop, long retryDelay)
        {
            _stop = stop;
            _retryDelay = retryDelay;
        }

        public static InitFailureStrategy RetryImmediately()
        {
            return new InitFailureStrategy(false, 0);
        }

        public static InitFailureStrategy RetryWithDelay(long ms)
        {
            return new InitFailureStrategy(false, ms);
        }

        public static InitFailureStrategy Stop()
        {
            return new InitFailureStrategy(true, 0);
        }

        public override string ToString()
        {
            return $"InitFailureStrategy(stop={_stop}, retryDelay={_retryDelay})";
        }

    }
}
