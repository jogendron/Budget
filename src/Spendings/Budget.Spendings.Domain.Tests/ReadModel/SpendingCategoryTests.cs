using AutoFixture;
using FluentAssertions;

using Budget.Spendings.Domain.ReadModel.Entities;

namespace Budget.Spendings.Domain.Tests.ReadModel;

public class SpendingCategoryTests
{
    private Fixture fixture;

    public SpendingCategoryTests()
    {
        fixture = new Fixture();
    }

    [Fact]
    public void ConstructorForNewCategories_InitializesProperties_Correctly()
    {
        //Arrange
        var id = fixture.Create<Guid>();
        var userId = fixture.Create<string>();
        var name = fixture.Create<string>();
        var createdOn = fixture.Create<DateTime>();
        var frequency = fixture.Create<Frequency>();
        var amount = fixture.Create<double>();
        var description = fixture.Create<string>();


        //Act
        var category = new SpendingCategory(
            id, 
            userId,
            name, 
            createdOn,
            null,
            null,
            frequency, 
            amount, 
            description
        );

        //Assert
        category.Id.Should().Be(id);
        category.UserId.Should().Be(userId);
        category.Name.Should().Be(name);
        category.CreatedOn.Should().Be(createdOn);
        category.Frequency.Should().Be(frequency);
        category.Amount.Should().Be(amount);
        category.Description.Should().Be(description);
    }

    [Fact]
    public void ConstructorForExistingCategories_InitializesProperties_Correctly()
    {
        //Arrange
        var id = fixture.Create<Guid>();
        var userId = fixture.Create<string>();
        var name = fixture.Create<string>();
        var createdOn = fixture.Create<DateTime>();
        var modifiedOn = fixture.Create<DateTime>();
        var closedOn = fixture.Create<DateTime>();
        var frequency = fixture.Create<Frequency>();
        var amount = fixture.Create<double>();
        var description = fixture.Create<string>();

        //Act
        var category = new SpendingCategory(
            id, 
            userId,
            name, 
            createdOn,
            modifiedOn,
            closedOn,
            frequency, 
            amount, 
            description
        );

        //Assert
        category.Id.Should().Be(id);
        category.UserId.Should().Be(userId);
        category.Name.Should().Be(name);
        category.CreatedOn.Should().Be(createdOn);
        category.ModifiedOn.Should().Be(modifiedOn);
        category.ClosedOn.Should().Be(closedOn);
        category.Frequency.Should().Be(frequency);
        category.Amount.Should().Be(amount);
        category.Description.Should().Be(description);
    }
}