using MediatR;
using Nut.MediatR.ServiceLike;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLikeSample.Sample.Notification
{
    [AsEventListener("Mediator.SampleEvent")]
    public class SampleEvent : INotification
    {
        public SampleEvent(string id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }
        
        public string Id { get; }

        public string Name { get; }

        public int Age { get; }
    }
}
