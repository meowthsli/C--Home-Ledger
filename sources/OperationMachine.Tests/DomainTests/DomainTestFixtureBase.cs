using NUnit.Framework;
using Microsoft.Practices.Unity;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Meowth.OperationMachine.Domain.Entities;

namespace Meowth.OperationMachine.Tests
{
    [TestFixture]
    public abstract class DomainTestFixtureBase
    {
        protected DomainTestFixtureBase()
        {
            Container = new UnityContainer();
            Container.RegisterInstance<IDomainEventBus>(new DomainEventBus());
            DomainEntity.SetEventRouter(Container.Resolve<IDomainEventBus>());
        }

        protected UnityContainer Container { get; private set; }
    }
}
