using System;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.DomainInfrastructure;

namespace Meowth.OperationMachine.Domain.Entities
{
    /// <summary>
    /// Abstract domain entity - can route domain events
    /// </summary>
    public abstract class DomainEntity : IAnyDomainEntity
    {
        public static void Subscribe<TEvent>(Action<TEvent> handler)
            where TEvent : class, IAnyDomainEvent
        {
            Bus.Register(handler);
        }

        protected static readonly EventRouter Bus = new EventRouter();
    }

    /// <summary> 
    /// Domain entity that routes events of specified type
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DomainEntity<TEntity> : DomainEntity
        where TEntity : class, IAnyDomainEntity
    {
        protected static void Publish(DomainEvent<TEntity> domainEvent)
        {
            Bus.Route(domainEvent);
        }
    }
}
