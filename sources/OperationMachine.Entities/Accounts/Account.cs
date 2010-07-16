using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Meowth.OperationMachine.Entities.Accounts
{
    #region message router

    public interface IRouteEvents
    {
        void Route(IAnyDomainEvent ev);
    }

    public class MessageRouter : IRouteEvents
    {
        private readonly IDictionary<Type, ICollection<Action<IAnyDomainEvent>>> _routes
            = new Dictionary<Type, ICollection<Action<IAnyDomainEvent>>>();

        private static readonly object Locker = new object();
        
        public void Register<TEvent>(Action<TEvent> route) 
            where TEvent : class, IAnyDomainEvent
        {
            lock (Locker)
            {
                var routingKey = typeof (TEvent);
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

    #endregion

    ///// <summary>
    ///// (Transactional?) wrapper to message router
    ///// </summary>
    //public class DirectBus : IBus
    //{
    //    public void Publish(IAnyDomainEvent ev)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void 
    //}

    public interface IAnyDomainEntity
    {
    }

    public abstract class DomainEntity : IAnyDomainEntity
    {
        public static void Subscribe<TEvent>(Action<TEvent> handler)
            where TEvent : class, IAnyDomainEvent
        {
            Bus.Register(handler);
        }

        protected static readonly MessageRouter Bus = new MessageRouter();
    }
    /// <summary> </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DomainEntity<TEntity> : DomainEntity
        where TEntity : class, IAnyDomainEntity
    {
        protected static void Publish(DomainEvent<TEntity> domainEvent)
        {
            Bus.Route(domainEvent);
        }
    }

    /// <summary> Active/Passive account </summary>
    public class Account : DomainEntity<Account>
    {
        public Account(string accountName, decimal initialAmount)
        {
            Name = accountName;
            Balance = initialAmount;
            PathName = AccountPathName.FromString(accountName);

            Publish(new AccountLifecycleEvent(this, EntityLifecycle.Created, false));
        }

        public Account(string accountName, decimal initialAmount, Account parent)
        {
            Name = accountName;
            Balance = initialAmount;
            PathName = AccountPathName.FromParentNameAndString(parent.PathName, accountName);

            Publish(new AccountLifecycleEvent(this, EntityLifecycle.Created, false));
        }

        protected Account()
        {
            // Publish(new AccountLifecycleEvent(this, EntityLifecycle.Created, false));
        }

        /// <summary> Identity </summary>
        public virtual Guid Id { get; protected set; }

        /// <summary> Name </summary>
        public virtual string Name { get; protected set; }

        /// <summary> Parent account </summary>
        public virtual Account Parent { get; protected set; }

        /// <summary> </summary>
        public virtual AccountPathName PathName { get; protected set; }

        /// <summary> Account balance </summary>
        public virtual decimal Balance { get; protected set; }

        /// <summary> </summary>
        public virtual decimal CreditTurnover { get; protected set; }

        /// <summary> </summary>
        public virtual decimal DebtTurnover { get; protected set; }

        /// <summary> </summary>
        public virtual DateTime Created { get; protected set; }

        public virtual decimal Turnover { get { return CreditTurnover + DebtTurnover; } }
        public virtual decimal Saldo { get { return Balance + (CreditTurnover - DebtTurnover); } }
        
        /// <summary> </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Account CreateSubaccount(string name)
        {
            var acc = new Account(name, 0.0m, this);
            Publish(new AccountLifecycleEvent(acc, EntityLifecycle.Created, true));
            return acc;
        }

        public virtual void TransactDebt(decimal amount)
        {
            DebtTurnover += amount;
            Publish(new TurnoverEvent(TurnoverType.Debt, amount));
        }

        public virtual void TransactCredit(decimal amount)
        {
            CreditTurnover += amount;
            Publish(new TurnoverEvent(TurnoverType.Credit, amount));
        }
    }

    public interface IAnyDomainEvent
    {
    }

    public class DomainEvent<TDomainEntity> : IAnyDomainEvent
    {
        private TDomainEntity _entity;
    }

    public enum EntityLifecycle
    {
        Created,
        Updated,
        Deleted,
    }

    public class EntityLifecycleEvent<T> : DomainEvent<T>
    {
        public EntityLifecycleEvent(T subject, EntityLifecycle lifeCycle)
        {
            Subject = subject;
            EntityLifecycle = lifeCycle;
        }

        public T Subject { get; private set; }

        public EntityLifecycle EntityLifecycle { get; private set; }
    }

    public class AccountLifecycleEvent : EntityLifecycleEvent<Account>
    {
        public AccountLifecycleEvent(Account subject, EntityLifecycle lifeCycle, bool isSubaccount)
            : base(subject, lifeCycle)
        {
            Subaccount = isSubaccount;
        }

        public bool Subaccount { get; private set; }
    }

    public enum TurnoverType
    {
        Debt,
        Credit,
    }

    public class TurnoverEvent : DomainEvent<Account>
    {
        public TurnoverEvent(TurnoverType type, decimal amount)
        {
            TurnoverType = type;
            Amount = amount;
        }

        public TurnoverType TurnoverType { get; private set; }
        public decimal Amount { get; private set; }
    }

    /// <summary>
    /// Transaction class
    /// </summary>
    public class Transaction
    {
        public static Transaction CreateTransaction(string name,
            Account source, Account destination, decimal amount)
        {
            // todo:
            return null;
        }

        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Account Source { get; set; }

        public virtual Account Destination { get; set; }

        public virtual decimal Sum { get; set; }

        public virtual DateTime Date { get; set; }
    }
}
