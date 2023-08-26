using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Domain.Entities;

using FluentAssertions;
using AutoFixture;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryCommandTests
{
    private Fixture _fixture;

    private string _userId;
    private string _name;
    private Frequency _frequency;
    private double _amount;
    private string _description;

    private CreateSpendingCategoryCommand _command;

    public CreateSpendingCategoryCommandTests()
    {
        _fixture = new Fixture();

        _userId = _fixture.Create<string>();
        _name = _fixture.Create<string>();
        _frequency = _fixture.Create<Frequency>();
        _amount = _fixture.Create<double>();
        _description = _fixture.Create<string>();

        _command = new CreateSpendingCategoryCommand(
            _userId,
            _name,
            _frequency,
            _amount,
            _description
        );
    }

    [Fact]
    public void Constructor_Assigns_Properties()
    {
        //Arrange

        //Act

        //Assert
        _userId.Should().Be(_userId);
        _name.Should().Be(_name);
        _frequency.Should().Be(_frequency);
        _amount.Should().Be(_amount);
        _description.Should().Be(_description);
    }

}