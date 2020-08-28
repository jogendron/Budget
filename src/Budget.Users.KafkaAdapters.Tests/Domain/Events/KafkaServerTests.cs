using Xunit;
using FluentAssertions;
using Budget.Users.KafkaAdapters.Domain.Events;

namespace Budget.Users.KafkaAdapters.Tests.Domain.Events
{
    public class KafkaServerTests
    {
        [Fact]
        public void Constructor_SetsAdressToEmpty_PortToMinus1()
        {
            //Arrange
            KafkaServer server = new KafkaServer();

            //Act

            //Assert
            server.Address.Should().BeEmpty();
            server.Port.Should().Be(-1);
        }

        [Fact]
        public void Address_CanBeSet()
        {
            //Arrange
            KafkaServer server = new KafkaServer();
            string address = "localhost";

            //Act
            server.Address = address;

            //Assert
            server.Address.Should().Be(address);
        }

        [Fact]
        public void Port_CanBeSet()
        {
            //Arrange
            KafkaServer server = new KafkaServer();
            int port = 92;

            //Act
            server.Port = port;

            //Assert
            server.Port.Should().Be(port);
        }

        [Fact]
        public void ToString_ConcatsAddressAndPort()
        {
            //Arrange
            string address = "localhost";
            int port = 92;
            string expectedString = $"{address}:{port}";
            var server = new KafkaServer() { Address = address, Port = port };

            //Act
            string serverString = server.ToString();

            //Assert
            serverString.Should().Be(expectedString);
        }
    }
}