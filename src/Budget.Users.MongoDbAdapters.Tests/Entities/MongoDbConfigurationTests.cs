using System;

using Xunit;
using FluentAssertions;
using AutoFixture;

using Budget.Users.MongoDbAdapters.Entities;

namespace Budget.Users.MongoDbAdapters.Tests.Entities
{
    public class MongoDbConfigurationTests
    {
        private Fixture fixture;

        public MongoDbConfigurationTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Constructor_SetsProperty_ToEmptyValues()
        {
            //Arrange
            var config = new MongoDbConfiguration();

            //Act

            //Assert
            config.Address.Should().BeEmpty();
            config.UserName.Should().BeEmpty();
            config.Password.Should().BeEmpty();
            config.Port.Should().BeLessThan(0);
            config.EnableTransactions.Should().BeFalse();
        }

        [Fact]
        public void GetConnectionString_WhenOnlyAddressIsSet_ReturnsValidString()
        {
            //Arrange
            var config = new MongoDbConfiguration() { Address = "127.0.0.1" };
            var expectedConnectionString = "mongodb://127.0.0.1";

            //Act
            var connectionString = config.GetConnectionString();

            //Assert
            connectionString.Should().Be(expectedConnectionString);
        }

        [Fact]
        public void GetConnectionString_WhenAddressAndPortAreSet_ReturnsValidString()
        {
            //Arrange
            var config = new MongoDbConfiguration() { Address = "127.0.0.1", Port = 42 };
            var expectedConnectionString = "mongodb://127.0.0.1:42";

            //Act
            var connectionString = config.GetConnectionString();

            //Assert
            connectionString.Should().Be(expectedConnectionString);
        }

        [Fact]
        public void GetConnectionString_WhenAllPropertiesSet_ReturnsValidString()
        {
            //Arrange
            var config = fixture.Create<MongoDbConfiguration>();

            var expectedConnectionString = 
                $"mongodb://{config.UserName}:{config.Password}@{config.Address}:{config.Port}";

            //Act
            var connectionString = config.GetConnectionString();

            //Assert
            connectionString.Should().Be(expectedConnectionString);
        }

        [Fact]
        public void GetConnectionString_ThrowsArgumentException_WhenUserNameIsSetWithoutPassword()
        {
            //Arrange
            var config = new MongoDbConfiguration() { UserName = "username", Address = "127.0.0.1" };

            //Act
            Action action = (() => config.GetConnectionString());

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetConnectionString_ThrowsArgumentException_WhenPasswordIsSetWithoutUserName()
        {
            //Arrange
            var config = new MongoDbConfiguration() { Password = "password", Address = "127.0.0.1" };

            //Act
            Action action = (() => config.GetConnectionString());

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetConnectionString_ThrowsArgumentException_WhenAddressIsEmpty()
        {
            //Arrange
            var config = fixture.Create<MongoDbConfiguration>();
            config.Address = string.Empty;

            //Act
            Action action = (() => config.GetConnectionString());

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

    }
}