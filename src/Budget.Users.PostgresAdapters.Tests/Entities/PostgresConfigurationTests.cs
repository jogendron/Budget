using System;
using Xunit;
using FluentAssertions;
using AutoFixture;

using Budget.Users.PostgresAdapters.Entities;

namespace Budget.Users.PostgresAdapters.Tests.Entities
{
    public class PostgresConfigurationTests
    {
        private Fixture fixture;

        public PostgresConfigurationTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Constructor_SetsEverything_ToEmpty()
        {
            //Arrange
            var config = new PostgresConfiguration();

            //Act

            //Assert
            config.Host.Should().BeEmpty();
            config.Port.Should().Be(-1);
            config.Username.Should().BeEmpty();
            config.Password.Should().BeEmpty();
            config.Database.Should().BeEmpty();
        }

        [Fact]
        public void GetConnectionString_IsValid_WhenAllPropertiesAreSet()
        {
            //Arrange
            var config = new PostgresConfiguration();
            config.Host = fixture.Create<string>();
            config.Port = fixture.Create<int>();
            config.Username = fixture.Create<string>();
            config.Password = fixture.Create<string>();
            config.Database = fixture.Create<string>();

            var expectedConnectionString = $"Host={config.Host};Port={config.Port};Username={config.Username};" + 
                $"Password={config.Password};Database={config.Database}";

            //Act
            var connectionString = config.GetConnectionString();

            //Assert
            connectionString.Should().Be(expectedConnectionString);
        }

        [Fact]
        public void GetConnectionString_ThrowsArgumentException_WhenHostIsMissing()
        {
            //Arrange
            var config = new PostgresConfiguration();
            config.Host = string.Empty;
            config.Port = fixture.Create<int>();
            config.Username = fixture.Create<string>();
            config.Password = fixture.Create<string>();
            config.Database = fixture.Create<string>();

            //Act
            Action action = (() => config.GetConnectionString());

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetConnectionString_IsValid_WhenPortIsMissing()
        {
            //Arrange
            var config = new PostgresConfiguration();
            config.Host = fixture.Create<string>();
            config.Port = -1;
            config.Username = fixture.Create<string>();
            config.Password = fixture.Create<string>();
            config.Database = fixture.Create<string>();

            var expectedConnectionString = $"Host={config.Host};Username={config.Username};" + 
                $"Password={config.Password};Database={config.Database}";

            //Act
            var connectionString = config.GetConnectionString();

            //Assert
            connectionString.Should().Be(expectedConnectionString);
        }

        [Fact]
        public void GetConnectionString_ThrowsArgumentException_WhenUsernameIsMissing()
        {
            //Arrange
            var config = new PostgresConfiguration();
            config.Host = fixture.Create<string>();
            config.Port = fixture.Create<int>();
            config.Username = string.Empty;;
            config.Password = fixture.Create<string>();
            config.Database = fixture.Create<string>();

            //Act
            Action action = (() => config.GetConnectionString());

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetConnectionString_ThrowArgumentException_WhenPasswordIsMissing()
        {
            //Arrange
            var config = new PostgresConfiguration();
            config.Host = fixture.Create<string>();
            config.Port = fixture.Create<int>();
            config.Username = fixture.Create<string>();
            config.Password = string.Empty;;
            config.Database = fixture.Create<string>();

            //Act
            Action action = (() => config.GetConnectionString());

            //Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void GetConnectionString_IsValid_WhenDatabasedIsMissing()
        {
            //Arrange
            var config = new PostgresConfiguration();
            config.Host = fixture.Create<string>();
            config.Port = fixture.Create<int>();
            config.Username = fixture.Create<string>();
            config.Password = fixture.Create<string>();
            config.Database = string.Empty;

            var expectedConnectionString = $"Host={config.Host};Port={config.Port};Username={config.Username};" + 
                $"Password={config.Password}";

            //Act
            var connectionString = config.GetConnectionString();

            //Assert
            connectionString.Should().Be(expectedConnectionString);
        }
    }
}