using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

public class Program
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
        // Configure log4net
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

        // Create the ASP.NET Core builder

        var builder = WebApplication.CreateBuilder(args);

        log.Debug("builder createed");
        // Add gRPC services
        builder.Services.AddGrpc();

        // Build the application
        var app = builder.Build();

         var faultServiceServer = new FaultServiceServer(50051); // Use your desired port
        faultServiceServer.Start();
        // (Optional) Add REST API endpoints if needed
        // app.MapGet("/", () => "Hello from my REST API!");

        // Start the application
        log.Warn("Warning This is running");
        app.Run();
    }
}
