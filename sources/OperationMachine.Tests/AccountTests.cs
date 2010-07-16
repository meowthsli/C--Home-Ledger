using NUnit.Framework;
using Meowth.OperationMachine.Entities.Accounts;

namespace Meowth.OperationMachine.Tests
{
    [TestFixture]
    public class AccountTests : DomainTestFixtureBase
    {
        [Test]
        public void WhenAccountCreatedNameIsCorrect()
        {
            var acc = new Account("expences", 0.0m);
            Assert.AreEqual(AccountPathName.FromString("expences"), acc.PathName);
        }

        [Test]
        public void WhenAccountCreatedEventGenerated()
        {
            EntityLifecycleEvent<Account> @event = null;
            DomainEntity.Subscribe<AccountLifecycleEvent>(x => { @event = x; });

            var acc = new Account("expence", 0.0m);
            Assert.IsNotNull(@event);
            Assert.AreEqual(EntityLifecycle.Created,@event.EntityLifecycle);
            Assert.AreEqual(acc, @event.Subject);
        }

        [Test]
        public void WhenSubaccountCreatedItIsCorrectAndEventIsGenerated()
        {
            AccountLifecycleEvent @event = null;
            DomainEntity.Subscribe<AccountLifecycleEvent>(x => { @event = x; });
            
            var acc = new Account("root", 0.0m);
            var acc2 = acc.CreateSubaccount("subroot");

            Assert.IsNotNull(@event);
            Assert.IsInstanceOf(typeof(AccountLifecycleEvent), @event);
            Assert.AreEqual(EntityLifecycle.Created, @event.EntityLifecycle);
            Assert.AreEqual(acc2, @event.Subject);
            Assert.IsTrue(@event.Subaccount);
        }

        [Test]
        public void WhenSubaccountCreatedItsPathNameIsCorrect()
        {
            var acc = new Account("root", 0.0m);
            var acc2 = acc.CreateSubaccount("subroot");
            Assert.AreEqual(
                AccountPathName.FromString("root.subroot"),
                acc2.PathName
                );
        }
    }
}
