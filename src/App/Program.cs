public class Program
{
    static void Main(string[] args)
    {
        // Remove When out of proof of conecpt phase
        MessageBusService messageBusService = null;
        FaultServiceClient faultServiceClient = null;
        WarehouseService warehouseService = null;
        //


        // Initialize orchestrator components
        try
        {
            messageBusService = new MessageBusService();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Could not start the messageBusService");
        }
        try
        {
            faultServiceClient = new FaultServiceClient();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Could not start the FaultServiceClient");
        }
        try
        {
            warehouseService = new WarehouseService();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Could not start the warehouseService");
        }

        // Start services
        try {
        Thread messageBusThread = new Thread(messageBusService.StartListening);
        messageBusThread.Start();

        }
        catch {

            Console.WriteLine("Could not start messageBusThread");
        }
        // Start FaultService Client in another thread
        try {
        Thread faultServiceThread = new Thread(async () => await faultServiceClient.ListenForFaultsAsync());
        faultServiceThread.Start();
        }
        catch {
        Console.WriteLine("Could not start faultServiceThread");

        }
        Console.WriteLine("Orchestrator is running. Press any key to exit.");
        Console.ReadKey();

        // Stop services on exit
    }
}
