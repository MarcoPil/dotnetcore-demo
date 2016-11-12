﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rabbitmq_demo
{
    public class Sender : ISender, IDisposable
    {
        private readonly string _exchange;

        IConnection _connection;
        IModel _channel;

        public event EventHandler<SendEventArgs> Send;

        public Sender(string exchange = "demo")
            : this(new ConnectionFactory { HostName = "localhost" },
                  exchange)
        {
        }

        public Sender(IConnectionFactory factory, string exchange)
        {
            _exchange = exchange;

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Topic);
        }

        public void Publish<T>(T input)
        {
            var routingKey = typeof(T).Name;

            var message = input.ToMessage();
            Send?.Invoke(this, new SendEventArgs { Topic = routingKey, Message = message });

            _channel.BasicPublish(exchange: _exchange,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: message.ToBody());
        }

        public void Dispose()
        {
            _connection.Dispose();
            _channel.Dispose();
        }
    }
}
