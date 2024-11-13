using Grpc.Core;
using FaultInterface; // Assuming this namespace contains the FaultService definition

public class FaultServiceServer
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FaultServiceServer));
    private readonly Grpc.Core.Server server;

    public FaultServiceServer(int port)
    {
        server = new Grpc.Core.Server
        {
            Services = { FaultService.BindService(new FaultServiceImpl()) },
            Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) } };
    }

    public void Start()
    {
        server.Start();
        log.Info($"FaultServiceServer started on port {server.Ports.First().BoundPort}");
    }

    public void Stop()
    {
        server.ShutdownAsync().Wait();
        log.Info("FaultServiceServer stopped.");
    }

    private class FaultServiceImpl : FaultService.FaultServiceBase
    {
        public override Task<FaultResponse> GetFaults(Empty request, ServerCallContext context)
        {
            log.Debug("Received request for faults");
            return Task.FromResult(new FaultResponse { /* Populate with fault data */ });
        }
    }
}
