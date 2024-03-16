using FluentAssertions;
using AutoFixture;

using Budget.EventSourcing.Events;
using Budget.Spendings.Domain.Events;
using Budget.Spendings.Domain.Factories;

namespace Budget.Spendings.Domain.Tests.WriteModel;

public class SpendingTests
{
    private Fixture fixture;
    private SpendingFactory factory;

    public SpendingTests()
    {
        fixture = new Fixture();

        factory = new SpendingFactory();
    }

    [Fact]
    public void Constructor_Adds_SpendingCreatedEvent()
    {
        //Arrange
        Guid categoryId = Guid.NewGuid();
        DateTime date = DateTime.Now;
        double amount = 10.5;
        string description = "dummy";

        //Act
        var spending = factory.Create(categoryId, date, amount, description);

        //Assert
        spending.NewChanges.Should().NotBeNullOrEmpty();
        spending.NewChanges.Count().Should().Be(1);
        spending.NewChanges.ElementAt(0).Should().BeOfType<SpendingCreated>();
    }

    [Fact]
    public void SpendingCreatedEvent_IsHandledCorrectly()
    {
        //Arrange
        var @event = fixture.Create<SpendingCreated>();

        //Act
        var spending = factory.Load(@event.AggregateId, new List<Event>() {
            @event
        });

        //Assert
        spending.Should().NotBeNull();
        spending.Id.Should().Be(@event.AggregateId);
        spending.CategoryId.Should().Be(@event.CategoryId);
        spending.Date.Should().Be(@event.Date);
        spending.Amount.Should().Be(@event.Amount);
        spending.Description.Should().Be(@event.Description);
    }

    [Fact]
    public void Update_Adds_SpendingUpdatedEvent()
    {
        //Arrange
        var @event = fixture.Create<SpendingCreated>();
        var spending = factory.Load(@event.AggregateId, new List<Event>() {
            @event
        });

        Guid categoryId = Guid.NewGuid();
        DateTime date = fixture.Create<DateTime>();
        double amount = 123;
        string description = fixture.Create<string>();

        //Act
        spending.Update(categoryId, date, amount, description);

        //Assert
        spending.NewChanges.Count().Should().Be(1);
        spending.NewChanges.First().Should().BeOfType<SpendingUpdated>();  
    }

    [Fact]
    public void SpendingUpdatedEvent_IsHandledCorrectly()
    {
        //Arrange
        var createEvent = fixture.Create<SpendingCreated>();
        var updateEvent = fixture.Build<SpendingUpdated>().With(
            e => e.Amount, 910
        ).Create();

        //Act
        var spending = factory.Load(createEvent.AggregateId, new List<Event>() {
            createEvent,
            updateEvent
        });

        //Assert
        spending.CategoryId.Should().Be(updateEvent.CategoryId);
        spending.Date.Should().Be(updateEvent.Date);
        spending.Amount.Should().Be(updateEvent.Amount);
        spending.Description.Should().Be(updateEvent.Description);
    }
}