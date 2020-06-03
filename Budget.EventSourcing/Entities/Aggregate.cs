using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Budget.EventSourcing.Events;
using Budget.EventSourcing.Exceptions;

namespace Budget.EventSourcing.Entities
{
    public abstract class Aggregate
    {
        private List<Event> changes;
        private List<Event> newChanges;

        protected Aggregate() : this(Guid.Empty, new List<Event>())
        {
        }

        protected Aggregate(Guid id, IEnumerable<Event> changes)
        {
            Id = id;
            this.changes = changes.ToList();
            this.newChanges = new List<Event>();
            EventHandlerMethods = new Dictionary<Type, MethodInfo>();
            
            MapEventHandlerMethods();
        }

        public Guid Id { get; protected set; }

        public IEnumerable<Event­­> Changes 
        { 
            get { return this.changes; } 
        }

        public IEnumerable<Event> NewChanges
        {
            get { return this.newChanges; }
        }

        private Dictionary<Type, MethodInfo> EventHandlerMethods { get; }      

        public void ApplyChangeHistory(DateTime? upTo = null)
        {
            DateTime maxDate = upTo.HasValue ? upTo.Value : DateTime.MaxValue;

            foreach (var change in Changes)
                if (change.IsBefore(maxDate))
                    ApplyChange(change);
        }

        protected void AddChange(Event @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            ApplyChange(@event);
            
            this.changes.Add(@event);
            this.newChanges.Add(@event);
        }

        private void ApplyChange(Event change) 
        {
            var type = change.GetType();

            if (! EventHandlerMethods.ContainsKey(change.GetType()))
               throw new EventNotHandledException(
                   $"Cannot apply unhandled event of type ${type.Name}"
                ); 

            var method = EventHandlerMethods[change.GetType()];

            try
            {
                method.Invoke(this, new[] { change });
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        private void MapEventHandlerMethods()
        {
            EventHandlerMethods.Clear();
            
            var eventHandlerInterfaces = this.GetType().GetInterfaces().Where(i => 
                i.IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
            );

            foreach (var i in eventHandlerInterfaces)
            {   
                var interfaceMap = this.GetType().GetInterfaceMap(i);
                var type = i.GetGenericArguments()[0];
                var method = interfaceMap.InterfaceMethods.Where(m => m.Name == "Handle").First();

                EventHandlerMethods.Add(type, method);
            }
        }

    }
}