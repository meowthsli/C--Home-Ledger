﻿using System;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.Events.Transactions;

namespace Meowth.OperationMachine.Domain.Entities.Transactions
{
    /// <summary>
    /// Transaction class
    /// </summary>
    public class Transaction
        : DomainEntity<Transaction>
    {
        public Transaction(string name, Account source, Account destination, decimal amount)
        {
            if (source == destination)
                throw new InvalidOperationException("acc1 == acc2");

            Name = name;
            Source = source;
            Destination = destination;
            Amount = amount;
            // TODO: date

            Publish(new EntityLifecycleEvent<Transaction>(this, EntityLifecyclePhase.Created));
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
            Publish(new TransactionExecutedEvent(this));
        }
    }

}