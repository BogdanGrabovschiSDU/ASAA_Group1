use amqprs::callbacks::DefaultChannelCallback;
use amqprs::callbacks::DefaultConnectionCallback;
use amqprs::channel::BasicConsumeArguments;
use amqprs::channel::BasicPublishArguments;
use amqprs::channel::QueueBindArguments;
use amqprs::channel::QueueDeclareArguments;
use amqprs::connection::Connection;
use amqprs::connection::OpenConnectionArguments;
use amqprs::BasicProperties;
#[tokio::main]
async fn main() {
    // Your asynchronous code here
    rabbit_test("test").await;
}
async fn rabbit_test(message: &str) -> bool {
    let connection = Connection::open(&OpenConnectionArguments::new(
        "localhost",
        5672,
        "guest",
        "guest",
    ))
    .await
    .unwrap();
    connection
        .register_callback(DefaultConnectionCallback)
        .await
        .unwrap();

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
            &queue_name,
            exchange_name,
            routing_key,
        ))
        .await
        .unwrap();

    // Start consumer
    let args = BasicConsumeArguments::new(&queue_name, "example_basic_pub_sub");
    let (_ctag, mut rx) = channel.basic_consume_rx(args).await.unwrap();

    tokio::spawn(async move {
        while let Some(msg) = rx.recv().await {
            if let Some(payload) = msg.content {
                println!(" [x] Received {:?}", (&payload));
            }
        }
    });

    println!(" [*] Waiting for messages. To exit press CTRL+C");

    println!(" [*] Waiting for messages. To exit press CTRL+C");
    // Publish message
    let content = String::from(
        r#"
        {
            "publisher": "example"
            "data": "Hello, amqprs!"
        }
    "#,
    )
    .into_bytes();

    let args = BasicPublishArguments::new(exchange_name, routing_key);
    channel
        .basic_publish(BasicProperties::default(), content, args)
        .await
        .unwrap();

    // Wait for the message to be received
    std::thread::sleep_ms(1000);
    channel.close().await.unwrap();
    connection.close().await.unwrap();
    let a = false;
    a
}
