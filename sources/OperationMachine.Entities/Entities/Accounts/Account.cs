﻿using System;
using System.Linq;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.Events.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure;

namespace Meowth.OperationMachine.Domain.Entities.Accounts
{
    /// <summary> 
    /// Active-passive account 
    /// </summary>
    public class Account : DomainEntity<Account>
    {
        public Account(string accountName)
        {
            Id = Guid.NewGuid();
            Name = accountName;
            PathName = AccountPathName.FromString(accountName);

            DomainEventBus.Route(new EntityCreatedEvent<Account>(this));
        }

        public Account(string accountName, Account parent)
        {
            Id = Guid.NewGuid();
            Name = accountName;
            PathName = AccountPathName.FromParentNameAndString(parent.PathName, accountName);
            Parent = parent;
            Level = parent.Level + 1;

            DomainEventBus.Route(new EntityCreatedEvent<Account>(this));
        }

        /// <summary>
        /// </summary>
        protected Account()
        {
        }

        #region properties

        /// <summary> Identity </summary>
        public virtual Guid Id { get; protected set; }

        public virtual int Level { get; protected set; }

        /// <summary> Name </summary>
        public virtual string Name { get; protected set; }

        /// <summary> Parent account </summary>
        public virtual Account Parent { get; protected set; }

        /// <summary> </summary>
        public virtual AccountPathName PathName { get; protected set; }

        /// <summary> Account balance </summary>
        public virtual decimal GetBalance()
        {
            return (CreditTurnover - DebtTurnover);
        }

        /// <summary> </summary>
        public virtual decimal CreditTurnover { get; protected set; }

        /// <summary> </summary>
        public virtual decimal DebtTurnover { get; protected set; }

        /// <summary> </summary>
        public virtual DateTime Created { get; protected set; }

        public virtual decimal GetTurnover()
        {
            return CreditTurnover + DebtTurnover;
        }

        public virtual bool IsSubaccount { get { return Parent != null; } }

        #endregion


        #region business logic

        /// <summary> Creates subaccount under this account </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Account CreateSubaccount(string name)
        {
            return new Account(name, this);
        }

        public virtual Account CreateSubaccountsTree(AccountPathName name)
        {
            var root = this;
            root = name.GetNameComponents().Aggregate(root, 
                (current, component) => current.CreateSubaccount(component));
            return root;
        }

        /// <summary>
        /// Transacts amount to debt part of account
        /// </summary>
        /// <param name="amount"></param>
        public virtual void TransactDebt(decimal amount)
        {
            DebtTurnover += amount;
            DomainEventBus.Route(new TurnoverEvent(TurnoverType.Debt, amount));

            if (Parent != null)
                Parent.TransactDebt(amount);
        }

        /// <summary>
        /// Transacts amount to credit part of account
        /// </summary>
        /// <param name="amount"></param>
        public virtual void TransactCredit(decimal amount)
        {
            CreditTurnover += amount;
            DomainEventBus.Route(new TurnoverEvent(TurnoverType.Credit, amount));

            if (Parent != null)
                Parent.TransactCredit(amount);
        }

        #endregion
    }
}