namespace Budget.Users.KafkaAdapters.Entities
{
    public class KafkaServer
    {
        public KafkaServer()
        {
            Address = string.Empty;
            Port = -1;
        }

        public KafkaServer(string address, int port)
        {
            Address = address;
            Port = port;
        }

        //Has public setter to be set by microsoft's configuration extension
        public string Address { get; set;}

        //Has public setter to be set by microsoft's configuration extension
        public int Port { get; set;}

        public override string ToString() 
        {
            return $"{Address}:{Port}";
        }
    }
}