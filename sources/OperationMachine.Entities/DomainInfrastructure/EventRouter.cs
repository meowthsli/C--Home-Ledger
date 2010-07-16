using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Meowth.OperationMachine.Domain.Events;

namespace Meowth.OperationMachine.Domain.DomainInfrastructure
{
    public class EventRouter : IRouteEvents
    {
        private readonly IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> _routes
            = new Dictionary<Type, ICollection<Action<IAnyDomainEvent>>>();

        private static readonly object Locker = new object();

        public void Register<TEvent>(Action<TEvent> route)
            where TEvent : class, IAnyDomainEvent
        {
            lock (Locker)
            {
                var routingKey = typeof(TEvent);
                ICollection<Action<IAnyDomainEvent>> routes;

                if (!_routes.TryGetValue(routingKey, out routes))
                    _routes[routingKey] = routes = new LinkedList<Action<IAnyDomainEvent>>();

                routes.Add(message => route(message as TEvent));
            }
        }

        public void Route(IAnyDomainEvent message)
        {
            ICollection<Action<IAnyDomainEvent>> routes;

            if (!_routes.TryGetValue(message.GetType(), out routes))
            {
                Trace.WriteLine("There is no route registered for message of type " + message.GetType());
                return;
            }

            lock (Locker)
            {
                foreach (var route in routes)
                    route(message);
            }
        }
    }

    public class RouteNotRegisteredException : Exception
    {
        public RouteNotRegisteredException(Type t)
        {
        }
    }
}
