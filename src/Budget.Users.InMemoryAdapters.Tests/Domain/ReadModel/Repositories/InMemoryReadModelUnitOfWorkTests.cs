using Xunit;
using NSubstitute;
using FluentAssertions;

using Budget.Users.Domain.ReadModel.Repositories;
using Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Tests.Domain.ReadModel.Repositories
{
    public class InMemoryReadModelUnitOfWorkTests
    {
        private IReadModelUserRepository repository;
        private InMemoryReadModelUnitOfWork unitOfWork;

        public InMemoryReadModelUnitOfWorkTests()
        {
            repository = Substitute.For<IReadModelUserRepository>();
            unitOfWork = new InMemoryReadModelUnitOfWork(repository);
        }

        [Fact]
        public void Constructor_Assigns_Repository()
        {
            //Arrange

            //Act

            //Assert
            unitOfWork.UserRepository.Should().Be(repository);
        }
    }
}