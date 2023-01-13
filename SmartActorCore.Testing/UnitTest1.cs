using SmartActorCore;
using System;

namespace SmartActorCore.Testing
{
    [TestClass]
    public class UnitTest1
    {
        public const string ROOT_DISPATCHER = "root-dispatcher";
        private const int _100K = 100 * 1024;

        private  volatile IActorSystem _actorSystem;
        


        private int parallelism;

        [TestInitialize]
        public void TestInitialize()
        {
            int cores = Environment.ProcessorCount;
            parallelism = Math.Max(2, cores / 2);
            SmartActorSystemSettings settings = new SmartActorSystemSettings(5, parallelism, 42);
            _actorSystem = new DefaultSmartActorSystem(settings);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _actorSystem.Stop();
            // _executor?.Cancel();
            
        }

        [TestMethod]
        public void Test1actorsAnd100KMessages()
        {
            //TODO

            //executor = ThingsBoardExecutors.newWorkStealingPool(parallelism, getClass());
          //  _actorSystem.CreateDispatcher(ROOT_DISPATCHER, executor);
            // testActorsAndMessages(1, _100K, 1);
        }

    }
}