using ConsoleActorBenchmark;
using Proto;
using Proto.Mailbox;

internal class Program
{
    private static void Main(string[] args)
    {
        // TestingPropertiesAndMiddleware.Execute();
        //  TestingContext.Execute();

        // protoactor_bootcamp.Execute();
        // protoactor_bootcamp.ExecutePoisonPill();
        protoactor_bootcamp.ExecuteBehavior();
    }


}