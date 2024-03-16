using Budget.Spendings.Application.Commands.CreateSpendingCategory;

using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryResponseTests
{
    public CreateSpendingCategoryResponseTests()
    {
    }

    [Fact]
    public void DefaultConstructor_SetsId_ToEmptyGuid()
    {
        //Arrange
        var expectedId = Guid.Empty;

        //Act
        var response = new CreateSpendingCategoryResponse();

        //Assert
        response.Id.Should().Be(expectedId);
    }

    [Fact]
    public void ParameteredConstructor_Assigns_Id()
    {
        //Arrange
        var expectedId = Guid.NewGuid();

        //Act
        var response = new CreateSpendingCategoryResponse(expectedId);

        //Assert
        response.Id.Should().Be(expectedId);
    }
}