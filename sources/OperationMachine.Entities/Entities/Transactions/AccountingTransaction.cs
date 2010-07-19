using System;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.Events.Transactions;
using Meowth.OperationMachine.Domain.DomainInfrastructure;

namespace Meowth.OperationMachine.Domain.Entities.Transactions
{
    /// <summary>
    /// Accounting transaction class
    /// </summary>
    public class AccountingTransaction : DomainEntity<AccountingTransaction>
    {
        public AccountingTransaction(string name, Account source, Account destination, decimal amount)
        {
            if (source == destination)
                throw new InvalidOperationException("acc1 == acc2");

            Id = Guid.NewGuid();
            Name = name;
            Source = source;
            Destination = destination;
            Amount = amount;
            // TODO: date

            DomainEventBus.Route(new EntityCreatedEvent<AccountingTransaction>(this));
        }

        public virtual Guid Id { get; protected set; }

        public virtual string Name { get; protected set; }

        public virtual Account Source { get; protected set; }

        public virtual Account Destination { get; protected set; }

        public virtual decimal Amount { get; protected set; }

        public virtual DateTime Date { get; protected set; }

        public virtual bool IsExecuted { get; protected set; }

        /// <summary>
        /// Execute transaction
        /// </summary>
        public virtual void Execute()
        {
            if (IsExecuted)
                throw new InvalidOperationException();

            Source.TransactDebt(Amount);
            Destination.TransactCredit(Amount);

            IsExecuted = true;
            DomainEventBus.Route(new AccountingTransactionExecutedEvent(this));
        }

        protected AccountingTransaction()
        {
            
        }
    }

}
