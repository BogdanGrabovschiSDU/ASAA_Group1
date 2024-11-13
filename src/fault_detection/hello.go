package main

import (
    "context"
    "fmt"
    "log"
    "time"

    "github.com/go-redis/redis/v8"
    "google.golang.org/grpc"
    "fault_dee" // Import the generated gRPC code
)

func main() {
    // Set up Redis client
    rdb := redis.NewClient(&redis.Options{
        Addr: "redis:6379",
    })
    ctx := context.Background()

    // Connect to Redis
    if _, err := rdb.Ping(ctx).Result(); err != nil {
        log.Fatalf("Failed to connect to Redis: %v", err)
    }

    // Set up gRPC client for FaultService
    grpcConn, err := grpc.Dial("other-service-address:50051", grpc.WithInsecure())
    if err != nil {
        log.Fatalf("Failed to connect to gRPC FaultService: %v", err)
    }
    defer grpcConn.Close()

    faultClient := faultinterface.NewFaultServiceClient(grpcConn)

    serviceName := "Service-1"
    checkInterval := 5 * time.Second

    for {
        // Get the last heartbeat timestamp
        value, err := rdb.Get(ctx, "heartbeat_timestamp").Result()
        if err != nil {
            log.Printf("Error retrieving heartbeat for %s: %v", serviceName, err)
            reportFault(faultClient, ctx, fmt.Sprintf("Failed to retrieve heartbeat: %v", err))
        } else {
            layout := "2006-01-02T15-04-05"
            lastHeartbeatTime, err := time.Parse(layout, value)
            if err != nil {
                log.Printf("Error parsing timestamp for %s: %v", serviceName, err)
                reportFault(faultClient, ctx, fmt.Sprintf("Failed to parse timestamp: %v", err))
            } else {
                elapsed := time.Since(lastHeartbeatTime)
                if elapsed > checkInterval {
                    faultMessage := fmt.Sprintf("Heartbeat missing for %v", elapsed)
                    reportFault(faultClient, ctx, faultMessage)
                    fmt.Printf("Alert: %s %s\n", serviceName, faultMessage)
                } else {
                    fmt.Printf("%s heartbeat is recent (elapsed: %v)\n", serviceName, elapsed)
                }
            }
        }
        time.Sleep(checkInterval)
    }
}

// reportFault sends a fault message to the FaultService
func reportFault(client faultinterface.FaultServiceClient, ctx context.Context, message string) {
    fault := &faultinterface.Fault{Message: message}
    faultResponse, err := client.GetFaults(ctx, &faultinterface.Empty{})
    if err != nil {
        log.Printf("Failed to report fault to gRPC FaultService: %v", err)
    } else {
        log.Printf("Fault reported: %s", fault.Message)
    }
}
