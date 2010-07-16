﻿using Meowth.OperationMachine.Domain.Accounts;

using Meowth.OperationMachine.Domain.Entities;
using Meowth.OperationMachine.Domain.Entities.Accounts;

using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.Events.Accounts;

using NUnit.Framework;

namespace Meowth.OperationMachine.Tests
{
    [TestFixture]
    public class AccountTests : DomainTestFixtureBase
    {
        [Test]
        public void WhenAccountCreatedEventGenerated()
        {
            EntityLifecycleEvent<Account> @event = null;
            DomainEntity.Subscribe<EntityLifecycleEvent<Account>>(x => { @event = x; });

            var acc = new Account("expence");
            Assert.IsNotNull(@event);
            Assert.AreEqual(EntityLifecyclePhase.Created, @event.EntityLifecycle);
            Assert.AreEqual(acc, @event.Subject);
        }

        [Test]
        public void WhenAccountCreatedNameIsCorrect()
        {
            var acc = new Account("expences");
            Assert.AreEqual(AccountPathName.FromString("expences"), acc.PathName);
        }

        [Test]
        public void WhenSubaccountCreatedItIsCorrectAndEventIsGenerated()
        {
            EntityLifecycleEvent<Account> @event = null;
            DomainEntity.Subscribe<EntityLifecycleEvent<Account>>(x => { @event = x; });

            var acc = new Account("root");
            Account acc2 = acc.CreateSubaccount("subroot");
            Assert.IsTrue(acc2.IsSubaccount);

            Assert.IsNotNull(@event);
            Assert.IsInstanceOf(typeof (EntityLifecycleEvent<Account>), @event);
            Assert.AreEqual(EntityLifecyclePhase.Created, @event.EntityLifecycle);
            Assert.AreEqual(acc2, @event.Subject);
        }

        [Test]
        public void WhenSubaccountCreatedItsPathNameIsCorrect()
        {
            var acc = new Account("root");
            Account acc2 = acc.CreateSubaccount("subroot");
            Assert.AreEqual(
                AccountPathName.FromString("root.subroot"),
                acc2.PathName
                );
        }

        [Test]
        public void WhenTransactionDebtOrCreditThenEventGenerated()
        {
            TurnoverEvent @event = null;
            DomainEntity.Subscribe<TurnoverEvent>(x => { @event = x; });

            var acc = new Account("root");
            acc.TransactDebt(0.0m);

            Assert.IsNotNull(@event);
            Assert.AreEqual(TurnoverType.Debt, @event.TurnoverType);

            acc.TransactCredit(0.0m);

            Assert.IsNotNull(@event);
            Assert.AreEqual(TurnoverType.Credit, @event.TurnoverType);
        }

        [Test]
        public void WhenTransactionCreditAndDebtThenAccountSaldoAndTurnoverAreOk()
        {
            var acc = new Account("root");
            
            acc.TransactCredit(10.0m);
            acc.TransactDebt(5.0m);

            Assert.AreEqual(10.0m, acc.CreditTurnover);
            Assert.AreEqual(5.0m, acc.DebtTurnover);

            Assert.AreEqual(15.0m, acc.Turnover);
            Assert.AreEqual(5.0, acc.Balance);
        }

        [Test]
        public void WhenTransactionCreditAndDebtThenParentAccountSaldoAndTurnoverAreOk()
        {
            var acc = new Account("root");
            var acc1 = acc.CreateSubaccount("expence");
            var acc2 = acc.CreateSubaccount("income");

            acc1.TransactDebt(6.0m);
            acc2.TransactCredit(17.0m);

            Assert.AreEqual(17.0m, acc.CreditTurnover);
            Assert.AreEqual(6.0m, acc.DebtTurnover);

            Assert.AreEqual(23.0m, acc.Turnover);
            Assert.AreEqual(11.0, acc.Balance);
        }
    }
}