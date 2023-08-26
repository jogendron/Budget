using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Budget.EventSourcing.Events;
using Budget.EventSourcing.Exceptions;

namespace Budget.EventSourcing.Entities;

public abstract class Aggregate
{
    private List<Event> _changes;
    private List<Event> _newChanges;
    private bool _isUpToDate;
    private Dictionary<Type, MethodInfo> _eventHandlerMethods;
    
    protected Aggregate() : this(Guid.Empty, new List<Event>())
    {
        _isUpToDate = true;
    }

    protected Aggregate(Guid id, IEnumerable<Event> changes)
    {
        Id = id;

        _changes = changes.ToList();
        _newChanges = new List<Event>();
        _isUpToDate = ! changes.Any();
        _eventHandlerMethods = new Dictionary<Type, MethodInfo>();
        
        MapEventHandlerMethods();
    }

    public Guid Id { get; protected set; }

    [IgnoreDataMember]
    [JsonIgnore]
    public IEnumerable<Event­­> Changes => this._changes;

    [IgnoreDataMember]
    [JsonIgnore]
    public IEnumerable<Event> NewChanges => this._newChanges;

    public void ApplyChangeHistory(DateTime? upTo = null)
    {
        DateTime maxDate = upTo.HasValue ? upTo.Value : DateTime.MaxValue;

        InitializeMembers();

        foreach (var change in Changes)
            if (change.HadHappenedAt(maxDate))
                ApplyChange(change);

        _isUpToDate = upTo is null
            || ! Changes.Any() 
            || Changes.Last().EventDate <= maxDate;
    }

    protected abstract void InitializeMembers();

    protected void AddChange(Event @event)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        if (! _isUpToDate)
            throw new InvalidOperationException("Changes can only be added to an up to date aggregate");

        ApplyChange(@event);
        
        this._changes.Add(@event);
        this._newChanges.Add(@event);
    }

    private void ApplyChange(Event change) 
    {
        var type = change.GetType();

        if (! _eventHandlerMethods.ContainsKey(type))
            throw new EventNotHandledException(
                $"Cannot apply unhandled event of type ${type.Name}"
            ); 

        var method = _eventHandlerMethods[type];

        try
        {
            method.Invoke(this, new[] { change });
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
                throw ex.InnerException;
            else
                throw;
        }
    }

    private void MapEventHandlerMethods()
    {
        _eventHandlerMethods.Clear();
        
        var eventHandlerInterfaces = this.GetType().GetInterfaces().Where(i => 
            i.IsGenericType
            && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
        );

        foreach (var i in eventHandlerInterfaces)
        {   
            var interfaceMap = this.GetType().GetInterfaceMap(i);
            var type = i.GetGenericArguments()[0];
            var method = interfaceMap.InterfaceMethods.Where(m => m.Name == "Handle").First();

            _eventHandlerMethods.Add(type, method);
        }
    }

}