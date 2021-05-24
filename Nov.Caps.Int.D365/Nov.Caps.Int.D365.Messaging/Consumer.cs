using Newtonsoft.Json;
using Nov.Caps.Int.D365.Messaging.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Messaging
{
    public class Consumer<TIn, TOut>
    {
        private readonly ILogger logger;
        private readonly JsonSerializer serializer;
        private readonly string queueName;
        private readonly IHandler<TIn, TOut> handler;

        public Consumer(ILogger logger, JsonSerializer serializer, string queueName, IHandler<TIn, TOut> handler)
        {
            this.logger = logger;
            this.serializer = serializer;
            this.queueName = queueName;
            this.handler = handler;
        }

        public ChannelReader<Result> Consume(IModel connection, CancellationToken cancellation)
        {
            var ch = Channel.CreateBounded<Result>(1);
            // set backpreassure on the connection
            connection.BasicQos(1, 1, false);

            Task.Run(() => {
                var consumer = new AsyncEventingBasicConsumer(connection);
                consumer.Received += async (_, ea) =>
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        ch.Writer.Complete();

                        this.logger.Information(
                            "Stopped consuming messages {queue}",
                            queueName
                        );

                        return;
                    }

                    Payload<TIn> payload;

                    this.logger.Information("Received a new message {queue} {routingKey} {@properties}", queueName, ea.RoutingKey, ea.BasicProperties);

                    // Try to deserialize body first
                    try
                    {
                        var body = ea.Body;
                        var str = System.Text.Encoding.UTF8.GetString(body.ToArray());

                        using (var sr = new StringReader(str))
                        {
                            payload = this.serializer.Deserialize<Payload<TIn>>(new JsonTextReader(sr));
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.Error("Failed to deserialize message body {@error} {queue}", e, queueName);

                        // Disgard the message, but do not requeue
                        connection.BasicNack(ea.DeliveryTag, false, false);

                        return;
                    }


                    // If body is successfully serialized
                    // Try to call handler
                    try
                    {
                        TOut output = await this.handler.Handle(payload);

                        var message = new Message<Payload<dynamic>>(
                            ea.BasicProperties.MessageId,
                            ea.BasicProperties.CorrelationId,
                            new Payload<dynamic>(payload.Metadata, output),
                            ea.BasicProperties.ReplyTo
                        );

                        await ch.Writer.WaitToWriteAsync();
                        await ch.Writer.WriteAsync(Result.Ok(message), cancellation);

                        this.logger.Information("Successfully handled message {queue}", queueName);

                        // Acknowledge the message and get ready for the next one
                        connection.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception e)
                    {
                        this.logger.Error("Failed to execute event handler {@message} {@error} {queue}", payload, e, queueName);

                        // Disgard the message, but do not requeue
                        connection.BasicNack(ea.DeliveryTag, false, false);

                        var failure = new Failure(
                            new Error(e.Message, e.InnerException),
                            new Payload<dynamic>(payload.Metadata, payload.Data)
                        );
                        var message = new Message<Failure>(
                            ea.BasicProperties.MessageId,
                            ea.BasicProperties.CorrelationId,
                            failure
                        );

                        await ch.Writer.WaitToWriteAsync();
                        await ch.Writer.WriteAsync(Result.Err(message), cancellation);
                    }
                };

                try
                {
                    connection.BasicConsume(queueName, false, consumer);
                    this.logger.Information("Started to consume messages from a queue {queue}", queueName);

                    cancellation.WaitHandle.WaitOne();

                    this.logger.Information("Received a cancellation signal {queue}", queueName);
                }
                catch (Exception e)
                {
                    this.logger.Error("Failed to consume messages from a queue {queue} {@error}", queueName, e);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }, cancellation);

            return ch;
        }
    }
}
