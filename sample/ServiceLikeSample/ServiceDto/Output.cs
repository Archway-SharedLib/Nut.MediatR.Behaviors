using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLikeSample.ServiceDto
{
    public class Output
    {
        public Output(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
