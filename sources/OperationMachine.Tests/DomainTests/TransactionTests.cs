using System;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using NUnit.Framework;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.Domain.Entities.Transactions;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Microsoft.Practices.Unity;

namespace Meowth.OperationMachine.Tests
{
    [TestFixture]
    public class TransactionTests : DomainTestFixtureBase
    {
        [Test]
        public void WhenTransactionCreatedThenEventGenerated()
        {
            EntityCreatedEvent<Transaction> @event = null;
            Container.Resolve<IDomainEventBus>()
                .RegisterThreaded<EntityCreatedEvent<Transaction>>(e => { @event = e; });
            
            var tx = new Transaction("tx1", 
                new Account("acc1"), 
                new Account("acc2"), 0.0m);
            Assert.IsNotNull(@event);
            Assert.AreEqual(tx, @event.Subject);
        }

        [Test]
        public void WhenTransactionExecutedOnPairedAccountsThenParametersCalculated()
        {
            var rootAccount = new Account("root");
            var accIncome = rootAccount.CreateSubaccount("income");
            var accOutcome = rootAccount.CreateSubaccount("expences");

            const decimal amount = 77.0m;

            var tx = new Transaction("tx1",
                accIncome,
                accOutcome, 
                amount);
            tx.Execute();

            Assert.AreEqual(0.0m, accIncome.CreditTurnover);
            Assert.AreEqual(amount, accIncome.DebtTurnover);
            Assert.AreEqual(-amount, accIncome.GetBalance());

            Assert.AreEqual(0.0m, accOutcome.DebtTurnover);
            Assert.AreEqual(amount, accOutcome.CreditTurnover);
            Assert.AreEqual(amount, accOutcome.GetBalance());

            Assert.AreEqual(0.0m, rootAccount.GetBalance());
            Assert.AreEqual(2 * amount, rootAccount.GetTurnover());
        }
        [Test]
        public void WhenTransactionExecutedOnhierarchicalAccountsThenParametersCalculated()
        {
            var rootAccount = new Account("root");
            var accIncome = rootAccount.CreateSubaccount("income")
                .CreateSubaccount("income2");
            var accOutcome = rootAccount.CreateSubaccount("expences");

            const decimal amount1 = 42.0m;
            const decimal amount2 = 77.0m;

            new Transaction("tx1",
                accIncome,
                accOutcome,
                amount1).Execute();

            new Transaction("tx2",
                accOutcome,
                accIncome,
                amount2).Execute();
            

            Assert.AreEqual(amount2, accIncome.CreditTurnover);
            Assert.AreEqual(amount1, accIncome.DebtTurnover);
            Assert.AreEqual(amount2-amount1, accIncome.GetBalance());

            Assert.AreEqual(amount2, accOutcome.DebtTurnover);
            Assert.AreEqual(amount1, accOutcome.CreditTurnover);
            Assert.AreEqual(amount1-amount2, accOutcome.GetBalance());

            Assert.AreEqual(0.0m, rootAccount.GetBalance());
            Assert.AreEqual(2* (amount1+amount2), rootAccount.GetTurnover());
        }

        [Test]
        public void WhenTransactionExecutedMoreThenOnceThenExceptionGenerated()
        {
            var tx = new Transaction("tx1",
                new Account("acc1"),
                new Account("acc2"), 0.0m);
            tx.Execute();
            Assert.Throws<InvalidOperationException>(tx.Execute);
        }

        [Test]
        public void WhenAccountsAreEqualThenExceptionIsGenerated()
        {
            var acc = new Account("root");
            Assert.Throws<InvalidOperationException>(() => new Transaction("tx", acc, acc, 0.0m));
        }
    }
}
