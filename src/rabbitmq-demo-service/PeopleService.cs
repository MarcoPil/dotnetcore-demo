﻿using ef_demo;
using rabbitmq_demo;
using System;

namespace rabbitmq_demo_service
{
    public class PeopleService : IReceive<CreatePerson>
    {
        private ISender _sender;
        DemoContext _context;

        public PeopleService(DemoContext context, ISender sender)
        {
            _context = context;
            _sender = sender;
        }

        public void Execute(CreatePerson command)
        {
            var person = new Person { FirstName = command.FirstName, LastName = command.LastName };
            _context.People.Add(person);
            _context.SaveChanges();

            _sender.PublishEvent(new PersonCreated
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName
            });
        }
    }
}