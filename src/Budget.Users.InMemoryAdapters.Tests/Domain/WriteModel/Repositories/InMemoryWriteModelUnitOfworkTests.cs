using Xunit;
using FluentAssertions;
using NSubstitute;

using Budget.Users.Domain.WriteModel.Repositories;
using Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Tests.Domain.WriteModel.Repositories
{
    public class InMemoryWriteModelUnitOfWorkTests
    {
        private IWriteModelUserRepository repository;

        private InMemoryWriteModelUnitOfWork unitOfWork;

        public InMemoryWriteModelUnitOfWorkTests()
        {
            repository = Substitute.For<IWriteModelUserRepository>();
            unitOfWork = new InMemoryWriteModelUnitOfWork(repository);
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