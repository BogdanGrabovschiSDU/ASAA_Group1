package main

import (
	"context"
	faults "example/hello/Protos"
	"flag"
	"fmt"
	"log"
	"time"

	pb "example/hello/Protos"

	"github.com/go-redis/redis/v8"
	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"
)

var (
	addr = flag.String("addr", "localhost:50051", "the address to connect to")
	name = flag.String("name", "world", "Name to greet")
)

func main() {
	flag.Parse()
	// Set up a connection to the server.
	conn, err := grpc.NewClient(*addr, grpc.WithTransportCredentials(insecure.NewCredentials()))
	if err != nil {
		log.Fatalf("did not connect: %v", err)
	}
	defer conn.Close()
	c := pb.NewFaultServiceClient(conn)

	// Contact the server and print out its response.
	ctx, cancel := context.WithTimeout(context.Background(), time.Second)
	defer cancel()
	r, err := c.GetFaults(ctx, &pb.Empty{})
	if err != nil {
		log.Fatalf("could not greet: %v", err)
	}
	log.Printf("Greeting: %s", r.GetFaults())

	// Set up Redis client
	rdb := redis.NewClient(&redis.Options{
		Addr: "redis:6379",
	})
	context.Background()

	// Connect to Redis
	if _, err := rdb.Ping(ctx).Result(); err != nil {
		log.Fatalf("Failed to connect to Redis: %v", err)
	}

	serviceName := "Service-1"
	checkInterval := 5 * time.Second

	for {
		// Get the last heartbeat timestamp
		value, err := rdb.Get(ctx, "heartbeat_timestamp").Result()
		if err != nil {
			log.Printf("Error retrieving heartbeat for %s: %v", serviceName, err)
		} else {
			layout := "2006-01-02T15-04-05"
			lastHeartbeatTime, err := time.Parse(layout, value)
			if err != nil {
				log.Printf("Error parsing timestamp for %s: %v", serviceName, err)
			} else {
				elapsed := time.Since(lastHeartbeatTime)
				if elapsed > checkInterval {
				} else {
					fmt.Printf("%s heartbeat is recent (elapsed: %v)\n", serviceName, elapsed)
				}
			}
		}
		time.Sleep(checkInterval)
	}
}

// reportFault sends a fault message to the FaultService
func reportFault(client faults.FaultServiceClient, ctx context.Context, message string) {
	fault := &faults.Fault{Message: message}
	_, err := client.GetFaults(ctx, &faults.Empty{})
	if err != nil {
		log.Printf("Failed to report fault to gRPC FaultService: %v", err)
	} else {
		log.Printf("Fault reported: %s", fault.Message)
	}
}
