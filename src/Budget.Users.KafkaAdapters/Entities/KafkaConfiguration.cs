using System.Collections.Generic;
using System.Linq;

namespace Budget.Users.KafkaAdapters.Entities
{
    public class KafkaConfiguration
    {
        public KafkaConfiguration()
        {
            BootstrapServers = new List<KafkaServer>();
        }

        //Has public setter to be set by microsoft's configuration extension
        public List<KafkaServer> BootstrapServers { get; set; }
        
        public string GetBootstrapServerString()
        {
            string bootstrapServerString = string.Empty;

            if (BootstrapServers.Any())
            {
                bootstrapServerString = BootstrapServers.Select(
                    server => server.ToString()
                ).Aggregate( (x,y) => $"{x},{y}");
            }
            
            return bootstrapServerString;
        }
    }
}