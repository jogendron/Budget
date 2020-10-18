using System;
using System.Linq;

namespace Budget.Users.Api.Entities
{
    public class Providers
    {
        private string events;

        public Providers()
        {
        }

        public string Events 
        {
            get { return events; }
            set {
                string[] supportedValues = {"InMemory", "Kafka"};

                if (! supportedValues.Contains(value))
                    throw new ArgumentException($"Value {value} is not supported for events provider.");

                events = value;
            }
        }
    }
}