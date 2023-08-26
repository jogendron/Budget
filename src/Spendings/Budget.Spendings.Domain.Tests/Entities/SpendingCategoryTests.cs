using Budget.Spendings.Domain.Events;
using Budget.Spendings.Domain.Entities;
using Budget.Spendings.Domain.Factories;
using Budget.EventSourcing.Events;

using FluentAssertions;
using AutoFixture;

//We disable CS8625, which warns about passing null to non nullable types
//because we are testing how the class behaves when it receives null
#pragma warning disable CS8625

namespace Budget.Spendings.Domain.Tests.Entities;

public class SpendingCategoryTests
{
    private Fixture fixture;
    private SpendingCategoryFactory factory;

    public SpendingCategoryTests()
    {
        fixture = new Fixture();
        factory = new SpendingCategoryFactory();
    }

    private SpendingCategory GetExistingSpendingCategory()
    {
        Guid id = Guid.NewGuid();
        List<Event> changes = new List<Event>() 
        {
            new SpendingCategoryCreated(
                id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                new Events.Period(
                    DateTime.MinValue
                ),
                Events.Frequency.Daily,
                15,
                fixture.Create<string>()
            )
        };

        return factory.Load(id, changes);
    }

    [Fact]
    public void Constructor_Adds_SpendingCategoryCreatedEvent()
    {
        //Arrange
        string userId = fixture.Create<string>();
        string name = fixture.Create<string>();
        Domain.Entities.Frequency frequency = 
            Domain.Entities.Frequency.Daily;
        double amount = 10;
        string description = fixture.Create<string>();

        //Act
        SpendingCategory category = factory.Create(
            userId,
            name,
            frequency,
            amount,
            description
        );

        //Assert
        category.Changes.Should().NotBeEmpty();
        category.Changes.Count().Should().Be(1);
        category.Changes.ElementAt(0).Should().BeOfType<SpendingCategoryCreated>();
        category.Changes.ElementAt(0).Should().NotBeNull();
        
        var evt = category.Changes.ElementAt(0) as SpendingCategoryCreated;
        evt.Should().NotBeNull();
        if (evt != null)
        {
            evt.AggregateId.Should().NotBeEmpty();
            evt.EventId.Should().NotBeEmpty();
            evt.EventDate.Should().BeAfter(DateTime.MinValue);
            evt.UserId.Should().Be(userId);
            evt.Name.Should().Be(name);
            evt.Period.Should().NotBeNull();
            evt.Frequency.Should().Be(Events.Frequency.Daily);
            evt.Amount.Should().Be(amount);
            evt.Description.Should().Be(description);
        }
    }

    [Fact]
    public void SpendingCategoryCreatedEvent_IsHandledCorrectly_WhenValid()
    {
        //Arrange
        string userId = fixture.Create<string>();
        string name = fixture.Create<string>();
        DateTime beginDate = DateTime.MinValue;
        DateTime endDate = beginDate.AddDays(1);
        Events.Frequency frequency = Events.Frequency.Daily;
        double amount = 10;
        string description = fixture.Create<string>();

        Guid id = Guid.NewGuid();
        List<Event> changes = new List<Event>() 
        {
            new SpendingCategoryCreated(
                id,
                userId,
                name,
                new Events.Period(
                    beginDate,
                    endDate
                ),
                Events.Frequency.Daily,
                amount,
                description
            )
        };

        //Act
        SpendingCategory category = factory.Load(id, changes);

        //Assert    
        category.Id.Should().NotBeEmpty();
        category.UserId.Should().Be(userId);
        category.Name.Should().Be(name);
        category.Period.Should().NotBeNull();
        category.Period.BeginDate.Should().Be(beginDate);
        category.Period.EndDate.Should().Be(endDate);
        category.Frequency.Should().Be(frequency.ToDomainFrequency());
        category.Amount.Should().Be(amount);
        category.Description.Should().Be(description);
    }

    [Fact]
    public void SpendingCategoryCreatedEvent_ThrowsArgumentException_WhenNameIsNullOrEmpty()
    {
        //Arrange
        string userId = fixture.Create<string>();
        string name = string.Empty;
        DateTime beginDate = DateTime.MinValue;
        DateTime endDate = beginDate.AddDays(1);
        Budget.Spendings.Domain.Entities.Frequency frequency = 
            Budget.Spendings.Domain.Entities.Frequency.Daily;
        double amount = 10;
        string description = fixture.Create<string>();

        //Act
        var action = (() => factory.Create(
            userId,
            name,
            frequency,
            amount,
            description
        ));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SpendingCategoryCreatedEvent_ThrowsArgumentNullException_WhenPeriodIsNull()
    {
        //Arrange
        Guid id = Guid.NewGuid();
        List<Event> changes = new List<Event>() {
            new SpendingCategoryCreated(
                id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                null,
                Events.Frequency.Daily,
                10,
                fixture.Create<string>()
            )
        };

        //Act
        var action = (() => factory.Load(id, changes));

        //Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SpendingCategoryCreatedEvent_ThrowsArgumentException_WhenAmountIsNegative()
    {
        //Arrange
        string userId = fixture.Create<string>();
        string name = fixture.Create<string>();
        DateTime beginDate = DateTime.MinValue;
        DateTime endDate = beginDate.AddDays(1);
        Budget.Spendings.Domain.Entities.Frequency frequency = 
            Budget.Spendings.Domain.Entities.Frequency.Daily;
        double amount = -10;
        string description = fixture.Create<string>();

        //Act
        var action = (() => factory.Create(
            userId,
            name,
            frequency,
            amount,
            description
        ));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SpendingCategoryCreatedEvent_ThrowsArgumentException_WhenDescriptionIsNullOrEmpty()
    {
        //Arrange
        string userId = fixture.Create<string>();
        string name = fixture.Create<string>();
        DateTime beginDate = DateTime.MinValue;
        DateTime endDate = beginDate.AddDays(1);
        Budget.Spendings.Domain.Entities.Frequency frequency = 
            Budget.Spendings.Domain.Entities.Frequency.Daily;
        double amount = 10;
        string description = string.Empty;

        //Act
        var action = (() => factory.Create(
            userId,
            name,
            frequency,
            amount,
            description
        ));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ClosePeriod_Adds_SpendingCategoryPeriodChangedEvent()
    {
        //Arrange
        SpendingCategory category = GetExistingSpendingCategory();

        //Act
        category.ClosePeriod();

        //Assert
        category.NewChanges.Should().NotBeEmpty();
        category.NewChanges.Count().Should().Be(1);
        category.NewChanges.ElementAt(0).Should().BeOfType<SpendingCategoryPeriodClosed>();
        category.NewChanges.ElementAt(0).Should().NotBeNull();

        var evt = category.NewChanges.ElementAt(0) as SpendingCategoryPeriodClosed;
        if (evt != null)
        {
            evt.EventId.Should().NotBeEmpty();
            evt.AggregateId.Should().Be(category.Id);
            evt.EventDate.Should().BeAfter(DateTime.MinValue);
            
            evt.EndDate.Should().BeAfter(DateTime.MinValue);
            evt.EndDate.Should().BeBefore(DateTime.MaxValue);
        }
    }

    [Fact]
    public void SpendingCategoryPeriodClosedEvent_IsHandledCorrectly_WhenValid()
    {
        //Arrange
        SpendingCategory category = GetExistingSpendingCategory();

        //Act
        category.ClosePeriod();

        //Assert
        category.Period.EndDate.Should().NotBeNull();
        category.Period.EndDate.Should().BeAfter(DateTime.MinValue);
        category.Period.EndDate.Should().BeBefore(DateTime.MaxValue);
    }

    [Fact]
    public void SpendingCategoryPeriodClosedEvent_ThrowsInvalidOperationException_WhenPeriodIsAlreadyClosed()
    {
        //Arrange
        SpendingCategory category = GetExistingSpendingCategory();
        category.ClosePeriod();

        //Act
        var action = (() => category.ClosePeriod());

        //Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SpendingCategoryPeriodClosedEvent_ThrowsArgumentException_WhenEndDateIsBeforeBeginDate()
    {
        //Arrange
        Guid id = Guid.NewGuid();
        List<Event> changes = new List<Event>() 
        {
            new SpendingCategoryCreated(
                id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                new Events.Period(
                    DateTime.Now
                ),
                Events.Frequency.Daily,
                15,
                fixture.Create<string>()
            ),
            new SpendingCategoryPeriodClosed(
                id,
                DateTime.MinValue
            )
        };

        //Act
        var action = (() => factory.Load(id, changes));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SpendingCategoryPeriodOpenedEvent_ThrowsInvalidOperationException_WhenPeriodIsAlreadyOpened()
    {
        Guid id = Guid.NewGuid();
        List<Event> changes = new List<Event>() 
        {
            new SpendingCategoryCreated(
                id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                new Events.Period(
                    DateTime.Now
                ),
                Events.Frequency.Daily,
                15,
                fixture.Create<string>()
            ),
            new SpendingCategoryPeriodOpened(
                id
            )
        };

        //Act
        var action = (() => factory.Load(id, changes));

        //Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SpendingCategoryPeriodOpenedEvent_RemovesPeriodEndDate()
    {
        Guid id = Guid.NewGuid();
        List<Event> changes = new List<Event>() 
        {
            new SpendingCategoryCreated(
                id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                new Events.Period(
                    DateTime.Now
                ),
                Events.Frequency.Daily,
                15,
                fixture.Create<string>()
            ),
            new SpendingCategoryPeriodClosed(
                id,
                DateTime.Now
            )
            ,
            new SpendingCategoryPeriodOpened(
                id
            )
        };

        //Act
        var category = factory.Load(id, changes);

        //Assert
        category.Should().NotBeNull();
        category.Period.EndDate.Should().BeNull();
    }

    [Fact]
    public void Update_ThrowsArgumentException_WhenNameIsNullOrEmpty()
    {
        //Arrange
        var frequency = fixture.Create<Budget.Spendings.Domain.Entities.Frequency>();
        var amount = fixture.Create<double>();
        var description = fixture.Create<string>();

        SpendingCategory category = GetExistingSpendingCategory();

        //Act
        var action = (() => category.Update(string.Empty, frequency, amount, description));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ThrowsArgumentException_WhenDescriptionIsNullOrEmpty()
    {
        //Arrange
        var name = fixture.Create<string>();
        var frequency = fixture.Create<Budget.Spendings.Domain.Entities.Frequency>();
        var amount = fixture.Create<double>();

        SpendingCategory category = GetExistingSpendingCategory();

        //Act
        var action = (() => category.Update(name, frequency, amount, string.Empty));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ChangesPropertiesCorrectly()
    {
        //Arrange
        var name = fixture.Create<string>();
        var frequency = fixture.Create<Budget.Spendings.Domain.Entities.Frequency>();
        var amount = fixture.Create<double>();
        var description = fixture.Create<string>();

        SpendingCategory category = GetExistingSpendingCategory();

        //Act
        category.Update(name, frequency, amount, description);

        //Assert
        category.Name.Should().Be(name);
        category.Frequency.Should().Be(frequency);
        category.Amount.Should().Be(amount);
        category.Description.Should().Be(description);
    }
}
