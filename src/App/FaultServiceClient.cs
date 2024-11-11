using Grpc.Core;
using FaultInterface;

public class FaultServiceClient
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FaultServiceClient));
    private readonly string _faultServiceUrl = "localhost";  // gRPC server address
    private readonly int _port = 50051;  // Example port

    public async Task ListenForFaultsAsync()
    {
        Channel channel = new Channel($"{_faultServiceUrl}:{_port}", ChannelCredentials.Insecure);
        var client = new FaultService.FaultServiceClient(channel);

        // Call a method from gRPC service to receive faults
        var faults = await client.GetFaultsAsync(new Empty());
        foreach (var fault in faults.Faults)
        {
            log.Error($"Fault Received {fault.Message}");
            Console.WriteLine($"Received Fault: {fault.Message}");
        }

        await channel.ShutdownAsync();
    }
}
