using Grpc.Core;
using FaultInterface;

public class FaultServiceClient
{
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
            Console.WriteLine($"Received Fault: {fault.Message}");
        }

        await channel.ShutdownAsync();
    }
}
