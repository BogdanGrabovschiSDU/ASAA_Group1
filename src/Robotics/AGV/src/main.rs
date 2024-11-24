use amqprs::callbacks::DefaultChannelCallback;
use amqprs::callbacks::DefaultConnectionCallback;
use amqprs::channel::BasicConsumeArguments;
use amqprs::channel::QueueBindArguments;
use amqprs::channel::QueueDeclareArguments;
use amqprs::connection::Connection;
use amqprs::connection::OpenConnectionArguments;
use amqprs::error::Error;
use log::{debug, error, info, warn};
use log4rs;
#[tokio::main]
async fn main() {
    log4rs::init_file("log4rs.yaml", Default::default()).unwrap();
    match rabbit_test().await {
        Ok(success) => {
            if success {
                info!("RabbitMQ test successful");
            } else {
                warn!("RabbitMQ test encountered issues");
            }
        }
        Err(err) => {
            error!("RabbitMQ test failed: {}", err);
        }
    }
}
async fn rabbit_test() -> Result<bool, Error> {
    let connection = Connection::open(&OpenConnectionArguments::new(
        "localhost",
        5672,
        "guest",
        "guest",
    ))
    .await?;
    connection
        .register_callback(DefaultConnectionCallback)
        .await?;
    info!("Connected to RabbitMQ");

    let channel = connection.open_channel(None).await.unwrap();
    channel
        .register_callback(DefaultChannelCallback)
        .await
        .unwrap();

    let (queue_name, _, _) = channel
        .queue_declare(QueueDeclareArguments::default())
        .await
        .unwrap()
        .unwrap();

    let routing_key = "amqprs.example";
    let exchange_name = "amq.topic";
    channel
        .queue_bind(QueueBindArguments::new(
            &"AGV",
            exchange_name,
            routing_key,
        ))
        .await
        .unwrap();
    debug!("Channel created");

    // Start consumer
    let args = BasicConsumeArguments::new(&queue_name, "example_basic_pub_sub");
    let (_ctag, mut rx) = channel.basic_consume_rx(args).await.unwrap();

    tokio::spawn(async move {
        while let Some(msg) = rx.recv().await {
            if let Some(payload) = msg.content {
                let payload_str = String::from_utf8_lossy(&payload);
                info!(" [x] Received {}", payload_str);
                info!("Moving to workstation");
                info!("Moved to workedstation");
                info!("Handed off to workstation");
            }
        }
    });

    println!(" [*] Waiting for messages. To exit press CTRL+C");
    // Wait for the message to be received
    tokio::signal::ctrl_c().await.unwrap();

    channel.close().await.unwrap();
    connection.close().await.unwrap();
    info!("Connection closed");
    Ok(true)
}
