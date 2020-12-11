using MediatR;
using Nut.MediatR.ServiceLike;
using System;
using System.Collections.Generic;
using System.Text;

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
