package main

import (
	"log"
	"time"

	"github.com/streadway/amqp"
)

// Service defines the structure for a monitored service
type Service struct {
	Name string
}

// Monitor establishes a connection to RabbitMQ and publishes heartbeats.
func Monitor(service Service, queueName string, interval time.Duration, timeout time.Duration) {
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

	err = ch.ExchangeDeclare(
		"heartbeats", // name
		"topic",      // type
		true,         // durable
		false,        // auto-deleted
		false,        // internal
		false,        // no-wait
		nil,          // arguments
	)
	if err != nil {
		log.Fatalf("Failed to open a channel: %v", err)
	}

}
