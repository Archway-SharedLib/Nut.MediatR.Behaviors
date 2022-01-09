using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Nut.MediatR.ServiceLike;

namespace ServiceLikeSample.ServiceDto
{
    public class Input
    {
        public Input(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
