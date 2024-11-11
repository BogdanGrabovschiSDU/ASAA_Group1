package main

import (
	"context"
	"fmt"
	logger "github.com/jeanphorn/log4go"
	"time"
    "log"
	"github.com/go-redis/redis/v8"
	"github.com/streadway/amqp"
)

func main() {
    // Setup logging
	logger.LoadConfiguration("./logConfig.json")
    logger.Info("TEST")
	// Set up Redis client
	rdb := redis.NewClient(&redis.Options{
		Addr: "redis:6379",
	})
	ctx := context.Background()

	// Connect to Redis
	if _, err := rdb.Ping(ctx).Result(); err != nil {
		log.Fatalf("Failed to connect to Redis: %v", err)
	}

	// Create a service instance
	service1 := Service{Name: "Service-1"}

	// Start monitoring heartbeats
	go Monitor(service1, service1.Name, 5*time.Second, 30*time.Second)

	// Set up the RabbitMQ consumer to receive heartbeats
	conn, err := amqp.Dial("amqp://guest:guest@rabbitmq:5672/")
	if err != nil {
		log.Fatalf("Failed to connect to RabbitMQ: %v", err)
	}
	defer conn.Close()

	ch, err := conn.Channel()
	if err != nil {
		log.Fatalf("Failed to open a channel: %v", err)
	}
	defer ch.Close()

	q, err := ch.QueueDeclare(
		service1.Name, // Queue name (matching the service name)
		false,         // Durable
		false,         // Delete when unused
		false,         // Exclusive
		false,         // No-wait
		nil,           // Arguments
	)
	if err != nil {
		log.Fatalf("Failed to declare a queue: %v", err)
	}

	err = ch.QueueBind(
		q.Name,       // queue name
		"123",        // routing key
		"heartbeats", // exchange
		false,
		nil)
	if err != nil {
		log.Fatalf("Failed to declare a queue: %v", err)
	}

	msgs, err := ch.Consume(
		q.Name, // Queue
		"",     // Consumer
		true,   // Auto-ack
		false,  // Exclusive
		false,  // No-local
		false,  // No-wait
		nil,    // Args
	)
	if err != nil {
		log.Fatalf("Failed to register a consumer: %v", err)
	}

	// Keep listening for heartbeats
	forever := make(chan bool)

	go func() {
		for msg := range msgs {
			log.Printf("{id: %s, time:%s}", msg.Body, time.Now().UTC().Format("2006-01-02T15-04-05"))
			logMessage := fmt.Sprintf("{id: %s, time:%s}", msg.Body, time.Now().UTC().Format("2006-01-02T15-04-05"))
			// Store in Redis
			err := rdb.Set(ctx, "heartbeat_timestamp", logMessage, 0).Err()
			if err != nil {
				log.Fatalf("Failed to log timestamp to Redis: %v", err)
			} else {
				log.Println("Logged timestamp to Redis successfully.")
			}
		}
	}()

	log.Printf("Waiting for heartbeats. To exit press CTRL+C")
	<-forever
}

// printLogs reads and prints the stored heartbeat log from Redis
func printLogs(ctx context.Context, rdb *redis.Client) {
	log.Println("Reading heartbeat log from Redis:")
	value, err := rdb.Get(ctx, "heartbeat_timestamp").Result()
	if err != nil {
		log.Printf("Failed to read from Redis: %v", err)
		return
	}
	log.Printf("Log from Redis: %s", value)
}
