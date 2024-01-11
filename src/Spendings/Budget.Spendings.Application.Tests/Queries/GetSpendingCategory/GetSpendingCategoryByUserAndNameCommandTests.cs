using Budget.Spendings.Application.Queries.GetSpendingCategory;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpendingCategory;

public class GetSpendingCategoryByUserAndNameCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingCategoryByUserAndNameCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_SetsPropperties()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();

        //Act
        var command = new GetSpendingCategoryByUserAndNameCommand(userId, name);

        //Assert
        command.UserId.Should().Be(userId);
        command.Name.Should().Be(name);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange
        var name = _fixture.Create<string>();

        //Act
        var actionNull = (() => new GetSpendingCategoryByUserAndNameCommand(null!, name));
        var actionEmpty = (() => new GetSpendingCategoryByUserAndNameCommand(string.Empty, name));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Constructor_ThrowsArgumentException_WhenNameIsNullOrEmpty()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        //Act
        var actionNull = (() => new GetSpendingCategoryByUserAndNameCommand(userId, null!));
        var actionEmpty = (() => new GetSpendingCategoryByUserAndNameCommand(userId, string.Empty));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }
}