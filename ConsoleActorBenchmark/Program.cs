using ConsoleActorBenchmark;
using Proto;
using Proto.Mailbox;
using System.Net.Sockets;
using System.Net;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // TestingPropertiesAndMiddleware.Execute();
        //  TestingContext.Execute();

        // protoactor_bootcamp.Execute();
        // protoactor_bootcamp.ExecutePoisonPill();
        // protoactor_bootcamp.ExecuteBehavior();
        // await protoactor_bootcamp.ExecuteHierarchyAsync();
        // await protoactor_bootcamp.ExecuteRouterPool();
        // await protoactor_bootcamp.ExecuteRouterGroup();
        // await protoactor_bootcamp.ExecuteEventStream();
        // await protoactor_bootcamp.ExecuteRemoteServer();
        // await protoactor_bootcamp.ExecutePersistenceActors();

        await protoactor_money_transfer_saga.Execute();
    }

 


}