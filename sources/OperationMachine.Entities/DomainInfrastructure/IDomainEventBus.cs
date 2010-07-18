using System;
using Meowth.OperationMachine.Domain.Events;

namespace Meowth.OperationMachine.Domain.DomainInfrastructure
{
    public interface IDomainEventBus
    {
        void Route(IAnyDomainEvent ev);

        void Register<TEvent>(Action<TEvent> route)
            where TEvent : class, IAnyDomainEvent;

        void RegisterThreaded<TEvent>(Action<TEvent> route)
            where TEvent : class, IAnyDomainEvent;

        void ClearThreaded();

        void ClearAll();
    }
}
