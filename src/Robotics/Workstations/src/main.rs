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
use serde::Deserialize;

#[derive(Debug, Deserialize, Clone, PartialEq)]
enum Configs {
    Painting,
    Welding,
}

#[derive(Debug, Deserialize, Clone, PartialEq)]
enum Models {
    MountainBike,
    CityBike,
    OneWheeler,
}

#[derive(Debug, Deserialize, Clone, PartialEq)]
struct Configuration {
    model: Models,
    configuration: Configs,
}
#[tokio::main]
async fn main() {
    log4rs::init_file("log4rs.yaml", Default::default()).unwrap();
    let config = Configuration {
        model: Models::MountainBike,
        configuration: Configs::Welding,
    };
    match start_rabbit_server(config).await {
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
fn reconfigure() {
    for n in 1..11 {
        std::thread::sleep_ms(1000);
        info!("{}% toolchange", n * 10);
    }
}
async fn start_rabbit_server(startConf: Configuration) -> Result<bool, Error> {
    let mut current_config = startConf;
    let connection = Connection::open(&OpenConnectionArguments::new(
        "rabbitmq", 5672, "guest", "guest",
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
            &"Workstation",
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
                println!(" [x] Received {:?}", payload_str);

                match serde_json::from_str::<Configuration>(&payload_str) {
                    Ok(new_config) => {
                        if new_config.model != current_config.model {
                            println!("Model changed! Reconfiguring...");
                            reconfigure();
                            current_config = new_config;
                        } else {
                            println!("Model is the same, no reconfiguration needed.");
                        }
                    }
                    Err(err) => {
                        println!("Error parsing JSON: {}", err);
                    }
                }
            }
        }
    });

    println!(" [*] Waiting for messages. To exit press CTRL+C");
    tokio::signal::ctrl_c().await.unwrap();

    // Wait for the message to be received
    std::thread::sleep_ms(1000);
    channel.close().await.unwrap();
    connection.close().await.unwrap();
    info!("Connection closed");
    Ok(true)
}
