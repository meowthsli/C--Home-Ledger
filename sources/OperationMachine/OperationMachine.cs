using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Microsoft.Practices.Unity;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Meowth.OperationMachine.Domain.Entities;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;

namespace Meowth.OperationMachine
{
    /// <summary> accounting calculator </summary>
    public class OperationMachine
    {
        /// <summary> In-memory database construction </summary>
        public OperationMachine()
        {
            DomainEntity.SetEventRouter(_container.Resolve<IDomainEventBus>());
        }

        private readonly UnityContainer _container = new UnityContainer();
    }
}
