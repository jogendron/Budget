using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Budget.Users.KafkaAdapters.Domain.Events;

namespace Budget.Users.KafkaAdapters.Tests.Domain.Events
{
    public class KafkaConfigurationTests
    {

        [Fact]
        public void Constructor_InitializesBootstrapServers()
        {
            //Arrange
            KafkaConfiguration config = new KafkaConfiguration();

            //Act

            //Assert
            config.BootstrapServers.Should().NotBeNull();
        }

        [Fact]
        public void BootstrapServers_CanBeSet()
        {
            //Arrange
            KafkaConfiguration config = new KafkaConfiguration();

            List<KafkaServer> servers = new List<KafkaServer>() {
                new KafkaServer() { Address = "localhost", Port = 92}
            };

            //Act
            config.BootstrapServers = servers;

            //Assert
            config.BootstrapServers.Should().BeEquivalentTo(servers);
        }

        [Fact]
        public void GetBootstrapServerString_ReturnsEmptyString_IfNoServerIsPresent()
        {
            //Arrange
            KafkaConfiguration config = new KafkaConfiguration();

            //Act
            string bootstrapServerString = config.GetBootstrapServerString();

            //Assert
            bootstrapServerString.Should().BeEmpty();
        }

        [Fact]
        public void GetBootstrapServerString_ReturnFirstServer_IfOnlyOneServerIsPresent()
        {
            //Arrange
            KafkaConfiguration config = new KafkaConfiguration();

            List<KafkaServer> servers = new List<KafkaServer>() {
                new KafkaServer() { Address = "localhost", Port = 92}
            };

            //Act
            config.BootstrapServers = servers;
            string bootstrapServerString = config.GetBootstrapServerString();

            //Assert
            bootstrapServerString.Should().Be(servers[0].ToString());
        }

        [Fact]
        public void GetBootstrapServerString_ConcatsServerString()
        {
            //Arrange
            KafkaConfiguration config = new KafkaConfiguration();

            List<KafkaServer> servers = new List<KafkaServer>() {
                new KafkaServer() { Address = "localhost", Port = 92},
                new KafkaServer() { Address = "::1", Port = 95}
            };

            string expectedString = $"{servers[0].ToString()},{servers[1].ToString()}";

            //Act
            config.BootstrapServers = servers;
            string bootstrapServerString = config.GetBootstrapServerString();

            //Assert
            bootstrapServerString.Should().Be(expectedString);
        }
        
    }
}