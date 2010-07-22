using Meowth.OperationMachine.CommandHandlers;
using Meowth.OperationMachine.Commands;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Meowth.OperationMachine.SessionManagement;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;

namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    [TestFixture]
    public class HandlerTest
    {
        public HandlerTest()
        {
            Container = new UnityContainer();
            Container.RegisterType<IDomainEventBus, DomainEventBusGate>();

            Container
                .RegisterType<IUnitOfWorkFactory, TestUoWFactory>()
                .RegisterType<IAccountRepository, TestAccountRepository>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<MakeTransactionCommandHandler, MakeTransactionCommandHandler>();
        }
     
        [Test]
        public void TestTransactionHandler()
        {
            var cmd = new MakeAccountingTransactionCommandDTO()
                          {
                              Amount = 5.0m,
                              Name = "tx1",
                              SourceAccountName = "root.subroot1",
                              DestinationAccountName = "root.subroot2"
                          };
            var hndl = Container.Resolve<MakeTransactionCommandHandler>();
            
            hndl.Handle(cmd);
            
            Assert.AreEqual(10.0, Container.Resolve<IAccountRepository>().GetRootAccount().GetTurnover());
        }

        [TestFixtureTearDown]
        protected void OnTearDown()
        {
            Container.Resolve<IDomainEventBus>().ClearAllSubscribers();
        }

        protected IUnityContainer Container { get; private set; }
    }
}
