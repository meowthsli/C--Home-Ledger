using System;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Microsoft.Practices.ServiceLocation;

namespace Meowth.OperationMachine.Domain.Entities
{
    /// <summary>
    /// Abstract domain entity - can route domain events
    /// </summary>
    public abstract class DomainEntity : IAnyDomainEntity
    {
        public static void SetEventRouter(IDomainEventBus bus)
        {
            Bus = bus;
        }

        protected void Publish<TEntity>(DomainEvent<TEntity> domainEvent)
            where TEntity : class, IAnyDomainEntity
        {
            Bus.Route(domainEvent);
        }

        protected static IDomainEventBus Bus;
    }
}
