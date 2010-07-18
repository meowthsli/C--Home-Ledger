using Meowth.OperationMachine.CommandHandlers;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Meowth.OperationMachine.SessionManagement;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities;

namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    [TestFixture]
    public class HandlerTest
    {
        public HandlerTest()
        {
            Container = new UnityContainer();
            Container.RegisterType<IDomainEventBus, DomainEventBus>();

            Container
                .RegisterInstance<IDomainEventBus>(new DomainEventBus())
                .RegisterType<IUnitOfWorkFactory, TestUoWFactory>()
                .RegisterInstance<IAccountRepository>(new TestAccountRepository())
                .RegisterType<MakeTransactionCommandHandler, MakeTransactionCommandHandler>();

            DomainEntity.SetEventRouter(Container.Resolve<IDomainEventBus>());
        }

        [Test]
        public void TestTransactionHandler()
        {
            var cmd = new MakeTransactionCommand()
                          {
                              Amount = 5.0m,
                              Name = "tx1",
                              SourceAccountName = "root.subroot1",
                              DestinationAccountName = "root.subroot2"
                          };
            var hndl = Container.Resolve<MakeTransactionCommandHandler>();
            hndl.Execute(cmd);

            Assert.AreEqual(10.0, Container.Resolve<IAccountRepository>().GetRootAccount().GetTurnover());
        }

        [TestFixtureTearDown]
        protected void OnTearDown()
        {
            Container.Resolve<IDomainEventBus>().ClearAll();
        }

        protected IUnityContainer Container { get; private set; }
    }
}
