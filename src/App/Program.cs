﻿using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System.Reflection;

public class Program
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
        // Configure log4net
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

        try
        {
            MessageBusService messageBusService = new();
            Thread messageBusThread = new Thread(messageBusService.StartListening);
            messageBusThread.Start();
        }
        catch (Exception)
        {
            log.Error("Could not connecto to the Rabbit");
        }
        var builder = WebApplication.CreateBuilder(args);

        log.Debug("builder created");
        builder.Services.AddGrpc();
        var app = builder.Build();
        var faultServiceServer = new FaultServiceServer(50051); // Use your desired port
        faultServiceServer.Start();
        app.MapPost("/", async (HttpContext context) =>
        {
            try
            {
                using var streamReader = new StreamReader(context.Request.Body);
                var json = await streamReader.ReadToEndAsync();
                log.Debug($"Received JSON: {json}");
                _ = new Order(json);
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("JSON received");
            }
            catch (JsonException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad Request");
            }
            catch (KeyNotFoundException)
            {
                log.Error("Either Model not given or the model is not available");
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad Request");

            }
            catch (Exception ex)
            {
                log.Error("Error Recieving JSON", ex);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Error processing JSON.");
            }
        });

        // Start the application
        log.Warn("Warning This is running");
        app.Run("http://localhost:8080");
    }
}
