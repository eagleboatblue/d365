using Newtonsoft.Json;
using Nov.Caps.Int.D365.Messaging.Models;
using RabbitMQ.Client;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Messaging
{
    public class Producer<T>
    {
        private readonly ILogger logger;
        private readonly JsonSerializer serializer;
        private readonly string exchangeName;
        private readonly string queueName;

        public Producer(ILogger logger, JsonSerializer serializer, string exchange) :
            this(logger, serializer, exchange, "")
        {
        }

        public Producer(ILogger logger, JsonSerializer serializer, string exchange, string queue)
        {
            this.logger = logger;
            this.serializer = serializer;
            this.exchangeName = exchange;
            this.queueName = queue;
        }

        public void Use(ChannelReader<Message<T>> input, IModel conn, CancellationToken token)
        {
            Task.Run(async () => {
                token.Register(() =>
                {
                    if (conn.IsOpen)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                });

                while (await input.WaitToReadAsync(token))
                {
                    if (token.IsCancellationRequested)
                    {
                        this.logger.Information(
                            "Stopped producing messages {exchange} {queue}",
                            this.exchangeName,
                            this.queueName
                        );

                        break;
                    }

                    var message = await input.ReadAsync(token);

                    try
                    {
                        using (var writer = new StringWriter())
                        {
                            this.serializer.Serialize(writer, message.Body);

                            var bytes = System.Text.Encoding.UTF8.GetBytes(writer.ToString());
                            conn.BasicPublish(this.exchangeName, this.queueName, true, null, bytes);

                            this.logger.Information(
                                "Published a message {exchange} {queue} {@message}",
                                this.exchangeName,
                                message.ReplyTo ?? this.queueName,
                                message.Body
                            );
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.Error(
                            "Failed to publish a message {error} {exchange} {queue} {@message}",
                            e,
                            this.exchangeName,
                            this.queueName,
                            message
                        );
                    }
                }
            }, token);
        }
    }
}
