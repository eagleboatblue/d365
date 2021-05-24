using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nov.Caps.Int.D365.Messaging.Models;
using RabbitMQ.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Nov.Caps.Int.D365.Messaging
{
    public class Router
    {
        private static readonly string failureExchangeName = "dead.letter.ex";
        private static readonly string resultExchangeName = "flow.ex";
        private static readonly string resultQueueName = "response";

        private bool running = false;
        private readonly ILogger logger;
        private readonly ConnectionFactory connectionFactory;
        private readonly JsonSerializer serializer;
        private readonly Dictionary<string, Func<IModel, CancellationToken, ChannelReader<Result>>> handlers;

        public Router(ILogger logger, Settings settings)
        {
            this.logger = logger;
            this.serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,

                DefaultValueHandling = DefaultValueHandling.Populate,

                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy
                    {
                        OverrideSpecifiedNames = false,
                        ProcessDictionaryKeys = true,
                        ProcessExtensionDataNames = true
                    }
                }
            };
            this.connectionFactory = new ConnectionFactory()
            {
                // Do not set to false if you aree using async consumers (as you should do)
                // https://gigi.nullneuron.net/gigilabs/asynchronous-rabbitmq-consumers-in-net/
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                HostName = settings.Url,
                UserName = settings.User,
                Password = settings.Password
            };
            this.connectionFactory.Ssl.Enabled = false;

            this.handlers = new Dictionary<string, Func<IModel, CancellationToken, ChannelReader<Result>>>();
        }

        public void AddHandler<TIn, TOut>(string queueName, IHandler<TIn, TOut> handler)
        {
            if (this.handlers.ContainsKey(queueName))
            {
                throw new InvalidOperationException($"Handler is already defined for '{queueName}' queue");
            }

            this.handlers[queueName] = (conn, token) =>
            {
                var consumer = new Consumer<TIn, TOut>(this.logger, this.serializer, queueName, handler);
                return consumer.Consume(conn, token);
            };
        }

        public Task Start(CancellationToken token)
        {
            lock (this)
            {
                if (this.running)
                {
                    throw new Exception("Manager is already running");
                }

                this.running = true;
            }

            IConnection connection = this.connectionFactory.CreateConnection();

            return Task.Run(() =>
            {
                using (connection)
                {
                    var failuresProducer = new Producer<Failure>(
                        this.logger,
                        this.serializer,
                        Router.failureExchangeName
                    );
                    var successProducer = new Producer<Payload<dynamic>>(
                        this.logger,
                        this.serializer,
                        Router.resultExchangeName,
                        Router.resultQueueName
                    );
                    var onSuccess = Channel.CreateUnbounded<Message<Payload<dynamic>>>();
                    var onFailure = Channel.CreateUnbounded<Message<Failure>>();

                    successProducer.Use(onSuccess.Reader, connection.CreateModel(), token);
                    failuresProducer.Use(onFailure.Reader, connection.CreateModel(), token);

                    async Task Redirect(ChannelReader<Result> input)
                    {
                        while (await input.WaitToReadAsync(token))
                        {
                            var result = await input.ReadAsync(token);

                            if (result.IsOk)
                            {
                                await onSuccess.Writer.WaitToWriteAsync();
                                await onSuccess.Writer.WriteAsync(result.Response);
                            }
                            else
                            {
                                await onFailure.Writer.WaitToWriteAsync();
                                await onFailure.Writer.WriteAsync(result.Failure);
                            }
                        }
                    }

                    try
                    {
                        this.handlers.Values.Select(i => Redirect(i(connection.CreateModel(), token))).ToArray();

                        this.logger.Information("Router has started");

                        token.WaitHandle.WaitOne();
                    }
                    catch (Exception e)
                    {
                        this.logger.Error("Failed to initialize handlers {@error}", e);

                        throw e;
                    }
                    finally
                    {
                        onSuccess.Writer.Complete();
                        onFailure.Writer.Complete();

                        lock (this)
                        {
                            if (this.running)
                            {
                                this.running = false;
                            }
                        }

                        this.logger.Information("Router has stopped");
                    }
                }
            }, token);
        }
    }
}
