using System;
using Xunit;
using AutoFixture;
using FluentAssertions;

using Budget.Users.Cryptography.Services;

namespace Budget.Users.Cryptography.Tests
{
    public class Sha512CryptServiceTests
    {
        private Fixture fixture;

        public Sha512CryptServiceTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Crypt_Returns_DifferentString()
        {
            //Arrange
            Sha512CryptService cryptService = new Sha512CryptService();
            string content = fixture.Create<string>();

            //Act
            string cryptedContent = cryptService.Crypt(content);

            //Assert
            cryptedContent.Should().NotBeNullOrEmpty();
            cryptedContent.Should().NotBe(content);
        }
    }
}
