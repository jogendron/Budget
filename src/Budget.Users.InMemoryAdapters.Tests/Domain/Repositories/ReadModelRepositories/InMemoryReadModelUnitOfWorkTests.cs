using Xunit;
using NSubstitute;
using FluentAssertions;

using Budget.Users.Domain.Repositories.ReadModelRepositories;
using Budget.Users.InMemoryAdapters.Domain.Repositories.ReadModelRepositories;

namespace Budget.Users.InMemoryAdapters.Tests.Domain.Repositories.ReadModelRepositories
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