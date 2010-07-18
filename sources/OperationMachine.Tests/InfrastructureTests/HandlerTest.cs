using Meowth.OperationMachine.CommandHandlers;
using Meowth.OperationMachine.Commands;
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
                .RegisterType<IAccountRepository, TestAccountRepository>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<MakeTransactionCommandHandler, MakeTransactionCommandHandler>();

            DomainEntity.SetEventRouter(Container.Resolve<IDomainEventBus>());
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
            
            using (var uof = Container.Resolve<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                using (var tx = uof.CreateTransaction())
                {
                    hndl.Execute(cmd);
                    tx.Commit();
                }
            }

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
