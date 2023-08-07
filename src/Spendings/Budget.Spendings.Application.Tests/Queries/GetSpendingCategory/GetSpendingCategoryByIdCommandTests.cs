using Budget.Spendings.Application.Queries.GetSpendingCategory;

using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpendingCategory;

public class GetSpendingCategoryByIdCommandTests
{
    public GetSpendingCategoryByIdCommandTests()
    {
        
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var command = new GetSpendingCategoryByIdCommand(id);

        //Assert
        command.Id.Should().Be(id);
    }
}