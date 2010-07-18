using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meowth.OperationMachine.Domain.Events;

namespace Meowth.OperationMachine.Domain.DomainInfrastructure
{
    public class EventRouter : IDomainEventBus
    {
        private IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> _commonRoutes
            = new Dictionary<Type, ICollection<Action<IAnyDomainEvent>>>();
        
        [ThreadStatic]
        private static IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> _routes;

        public void Register<TEvent>(Action<TEvent> route)
            where TEvent : class, IAnyDomainEvent
        {
            var routingKey = typeof(TEvent);
            ICollection<Action<IAnyDomainEvent>> routes;

        
            if (!_commonRoutes.TryGetValue(routingKey, out routes))
                _commonRoutes[routingKey] = routes = new LinkedList<Action<IAnyDomainEvent>>();

            routes.Add((message) => route(message as TEvent));
        }

        public void RegisterThreaded<TEvent>(Action<TEvent> route) where TEvent : class, IAnyDomainEvent
        {
            var routingKey = typeof(TEvent);
            ICollection<Action<IAnyDomainEvent>> routes;

            if (!GetRoutes().TryGetValue(routingKey, out routes))
                GetRoutes()[routingKey] = routes = new LinkedList<Action<IAnyDomainEvent>>();

            routes.Add((message) => route(message as TEvent));
        }

        private static IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> GetRoutes()
        {
            return _routes ?? (_routes = new Dictionary<Type, ICollection<Action<IAnyDomainEvent>>>());
        }

        public void Route(IAnyDomainEvent message)
        {
            ICollection<Action<IAnyDomainEvent>> routes;

            if (!_commonRoutes.TryGetValue(message.GetType(), out routes))
            {
                Trace.WriteLine("There is no route registered for message of type " + message.GetType());
            }
            else
                foreach (var route in routes)
                    route(message);

            if (!GetRoutes().TryGetValue(message.GetType(), out routes))
            {
                Trace.WriteLine("There is no threaded route registered for message of type " + message.GetType());
            }
            else
                foreach (var route in routes)
                    route(message);
        }

        public void ClearThreaded()
        {
            GetRoutes().Clear();
        }

        public void ClearAll()
        {
            ClearThreaded();
            _commonRoutes.Clear();
        }
    }
}
