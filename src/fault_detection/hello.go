package main

import (
    "fmt"
    "log"
    "time"

    "github.com/streadway/amqp"
)

// Define the Service struct
type Service struct {
    Name string
}

// Monitor establishes a connection to RabbitMQ and publishes heartbeats.
func Monitor(service Service, queueName string, interval time.Duration, timeout time.Duration) {
    conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/")
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
        queueName, // Queue name
        false,     // Durable
        false,     // Delete when unused
        false,     // Exclusive
        false,     // No-wait
        nil,       // Arguments
    )
    if err != nil {
        log.Fatalf("Failed to declare a queue: %v", err)
    }

    ticker := time.NewTicker(interval)
    defer ticker.Stop()

    timeoutTimer := time.NewTimer(timeout)
    defer timeoutTimer.Stop()

    for {
        select {
        case <-ticker.C:
            heartbeatMsg := fmt.Sprintf("Heartbeat from %s", service.Name)
            err = ch.Publish(
                "",     // Exchange
                q.Name, // Routing key (queue name)
                false,  // Mandatory
                false,  // Immediate
                amqp.Publishing{
                    ContentType: "text/plain",
                    Body:        []byte(heartbeatMsg),
                })
            if err != nil {
                log.Printf("Failed to publish a message: %v", err)
            } else {
                log.Printf("Sent: %s", heartbeatMsg)
            }
        case <-timeoutTimer.C:
            log.Printf("Timeout reached for service: %s", service.Name)
            return
        }
    }
}

func main() {
    // Create a service instance
    service1 := Service{Name: "Service-1"}

    // Start monitoring the service heartbeat in a goroutine
    //go Monitor(service1, service1.Name, 2*time.Second, 10*time.Second)

    // Set up the RabbitMQ consumer to receive heartbeats
    conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/")
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
            log.Printf("Received a heartbeat: %s", msg.Body)
        }
    }()

    log.Printf("Waiting for heartbeats. To exit press CTRL+C")
    <-forever
}
