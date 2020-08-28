namespace Budget.Users.KafkaAdapters.Domain.Events
{
    public class KafkaServer
    {
        public KafkaServer()
        {
            Address = string.Empty;
            Port = -1;
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