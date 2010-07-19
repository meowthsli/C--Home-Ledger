using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meowth.OperationMachine.Domain.Events;

namespace Meowth.OperationMachine.Domain.DomainInfrastructure
{
    /// <summary> Event bus for domain event </summary>
    internal static class DomainEventBus
    {
        private static readonly IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> CommonRoutes
            = new Dictionary<Type, ICollection<Action<IAnyDomainEvent>>>();

        private static readonly object Locker = new object();

        [ThreadStatic]
        private static IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> _routes;

        internal static void Register<TEvent>(Action<TEvent> route)
            where TEvent : class, IAnyDomainEvent
        {
            var routingKey = typeof(TEvent);
            ICollection<Action<IAnyDomainEvent>> routes;

            lock (Locker)
            {
                if (!CommonRoutes.TryGetValue(routingKey, out routes))
                    CommonRoutes[routingKey] = routes = new LinkedList<Action<IAnyDomainEvent>>();
            }

            routes.Add(message => route(message as TEvent));
        }   

        internal static void RegisterThreaded<TEvent>(Action<TEvent> route) where TEvent : class, IAnyDomainEvent
        {
            var routingKey = typeof(TEvent);
            ICollection<Action<IAnyDomainEvent>> routes;

            if (!GetRoutes().TryGetValue(routingKey, out routes))
                GetRoutes()[routingKey] = routes = new LinkedList<Action<IAnyDomainEvent>>();

            routes.Add(message => route(message as TEvent));
        }

        internal static IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> GetRoutes()
        {
            return _routes ?? (_routes = new Dictionary<Type, ICollection<Action<IAnyDomainEvent>>>());
        }

        internal static void Route(IAnyDomainEvent message)
        {
            ICollection<Action<IAnyDomainEvent>> routes = null;

            lock (Locker)
            {
                ICollection<Action<IAnyDomainEvent>> r;
                if (!CommonRoutes.TryGetValue(message.GetType(), out r))
                {
                    Trace.WriteLine("There is no route registered for message of type " + message.GetType());
                }
                else
                {
                    routes = new List<Action<IAnyDomainEvent>>(r);
                }
            }

            if (routes != null)
                foreach (var route in routes )
                    route(message);
            

            if (!GetRoutes().TryGetValue(message.GetType(), out routes))
            {
                Trace.WriteLine("There is no threaded route registered for message of type " + message.GetType());
            }
            else
                foreach (var route in routes)
                    route(message);
        }

        internal static void ClearThreadedSubscribers()
        {
            GetRoutes().Clear();
        }

        internal static void ClearAllSubscribers()
        {
            ClearThreadedSubscribers();
            lock(Locker)
                CommonRoutes.Clear();
        }
    }

    public class DomainEventBusGate : IDomainEventBus
    {
        public void Route(IAnyDomainEvent ev)
        {
            DomainEventBus.Route(ev);
        }

        public void Register<TEvent>(Action<TEvent> route) where TEvent : class, IAnyDomainEvent
        {
            DomainEventBus.Register(route);
        }

        public void RegisterThreaded<TEvent>(Action<TEvent> route) where TEvent : class, IAnyDomainEvent
        {
            DomainEventBus.RegisterThreaded(route);
        }

        public void ClearThreadedSubscribers()
        {
            DomainEventBus.ClearThreadedSubscribers();
        }

        public void ClearAllSubscribers()
        {
            DomainEventBus.ClearAllSubscribers();
        }
    }
}
