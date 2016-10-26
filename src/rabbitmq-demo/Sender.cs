﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rabbitmq_demo
{
    public class Sender : IDisposable
    {
        private readonly string _hostname;
        private readonly string _exchange;

        IConnection _connection;
        IModel _channel;

        public Sender(string hostname = "localhost", string exchange = "demo")
        {
            _hostname = hostname;
            _exchange = exchange;

            var factory = new ConnectionFactory() { HostName = _hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Fanout);
        }

        public void Publish<T>(T input)
        {
            var routingKey = typeof(T).FullName;

            var message = JsonConvert.SerializeObject(input);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: _exchange,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }

        public void Dispose()
        {
            _connection.Dispose();
            _channel.Dispose();
        }
    }
}