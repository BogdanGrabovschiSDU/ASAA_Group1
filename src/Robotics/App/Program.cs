using System;
using System.Threading;

public class Program
{
    static void Main(string[] args)
    {
        // Initialize orchestrator components
        var messageBusService = new MessageBusService();
        var faultServiceClient = new FaultServiceClient();
        var warehouseService = new WarehouseService();

        // Start services
        Thread messageBusThread = new Thread(messageBusService.StartListening);
        messageBusThread.Start();

        // Start FaultService Client in another thread
        Thread faultServiceThread = new Thread(async () => await faultServiceClient.ListenForFaultsAsync());
        faultServiceThread.Start();

        Console.WriteLine("Orchestrator is running. Press any key to exit.");
        Console.ReadKey();

        // Stop services on exit
        messageBusService.Stop();
    }
}
