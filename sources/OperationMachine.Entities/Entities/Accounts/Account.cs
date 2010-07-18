using System;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.Events.Accounts;

namespace Meowth.OperationMachine.Domain.Entities.Accounts
{
    /// <summary> 
    /// Active-passive account 
    /// </summary>
    public class Account : DomainEntity
    {
        public Account(string accountName)
        {
            Id = Guid.NewGuid();
            Name = accountName;
            PathName = AccountPathName.FromString(accountName);

            Publish(new EntityCreatedEvent<Account>(this));
        }

        public Account(string accountName, Account parent)
        {
            Name = accountName;
            PathName = AccountPathName.FromParentNameAndString(parent.PathName, accountName);
            Parent = parent;

            Publish(new EntityCreatedEvent<Account>(this));
        }

        /// <summary>
        /// </summary>
        protected Account()
        {
        }

        #region properties

        /// <summary> Identity </summary>
        public virtual Guid Id { get; protected set; }

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Transacts amount to debt part of account
        /// </summary>
        /// <param name="amount"></param>
        public virtual void TransactDebt(decimal amount)
        {
            DebtTurnover += amount;
            Publish(new TurnoverEvent(TurnoverType.Debt, amount));

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
            Publish(new TurnoverEvent(TurnoverType.Credit, amount));

            if (Parent != null)
                Parent.TransactCredit(amount);
        }

        #endregion
    }
}