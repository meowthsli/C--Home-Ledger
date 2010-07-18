using System;
using Meowth.OperationMachine.Domain.Events;

namespace Meowth.OperationMachine.Domain.DomainInfrastructure
{
    /// <summary> Bus to listen to the domain events </summary>
    public interface IDomainEventBus
    {
        /// <summary> </summary>
        /// <param name="ev"></param>
        void Route(IAnyDomainEvent ev);

        /// <summary> Register handler to listen to specified event </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="route"></param>
        void Register<TEvent>(Action<TEvent> route)
            where TEvent : class, IAnyDomainEvent;

#warning shoud be rewritten to aclually safe implementation
        /// <summary> Register threader (temporary) handler to listen to specified event </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="route"></param>
        void RegisterThreaded<TEvent>(Action<TEvent> route)
            where TEvent : class, IAnyDomainEvent;

#warning shoud be rewritten to aclually safe implementation
        /// <summary> Removes only this thread-installed subscribers </summary>
        void ClearThreadedSubscribers();

        /// <summary> Removes all subscribers </summary>
        void ClearAllSubscribers();
    }
}
