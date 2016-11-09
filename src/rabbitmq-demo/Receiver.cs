﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace rabbitmq_demo
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class Receiver : IReceiver, IDisposable
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private readonly string _exchange;

        IConnection connection;
        IModel channel;

        public Receiver(string exchange = "demo")
            : this(new ConnectionFactory() { HostName = "localhost" },
                  exchange)
        {
        }

        public Receiver(IConnectionFactory factory, string exchange)
        {
            _exchange = exchange;

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Topic);
        }

        public void Dispose()
        {
            connection.Dispose();
            channel.Dispose();
        }

        [Obsolete("Subscribe a class that implements the IReceive interface instead.")]
        public void Subscribe<T>(Action<T> action)
        {
            var routingkey = typeof(T).Name;

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: _exchange,
                              routingKey: routingkey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => action(Convert<T>(ea.Body));

            channel.BasicConsume(queue: queueName,
                             noAck: true,
                             consumer: consumer);
        }

        public void Subscribe<T>(IReceive<T> receiver)
        {
            var routingkey = typeof(T).Name;

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: _exchange,
                              routingKey: routingkey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) => receiver.Execute(Convert<T>(ea.Body));

            channel.BasicConsume(queue: queueName,
                             noAck: true,
                             consumer: consumer);
        }

        [Obsolete("Subscribe a ReceiverTask instead.")]
        public T WaitForResult<T>(Action publish, TimeSpan timeout)
        {
            T result = default(T);
            using (var wait = new ManualResetEvent(false))
            {
                Subscribe<T>(p => { result = p; wait.Set(); });
                publish();

                if (!wait.WaitOne(timeout))
                {
                    throw new TimeoutException();
                }
            }

            return result;
        }

        [Obsolete("Subscribe a ReceiverTask instead.")]
        public T WaitForResult<T>(Action publish)
        {
            return WaitForResult<T>(publish, TimeSpan.FromSeconds(5));
        }

        private static T Convert<T>(byte[] body)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
        }
    }
}
