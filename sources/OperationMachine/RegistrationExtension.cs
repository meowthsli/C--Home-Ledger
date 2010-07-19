using Meowth.OperationMachine.Domain.Events;
using Microsoft.Practices.Unity;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Entities.Transactions;

namespace Meowth.OperationMachine
{
    /// <summary>
    /// Extension to register domain
    /// </summary>
    public sealed class RegistrationExtension : UnityContainerExtension
    {
        /// <summary> Registers all mandatory handlers </summary>
        protected override void Initialize()
        {
            Bus.Register<EntityCreatedEvent<Account>>(
                (@event) => 
                    Container.Resolve<IAccountRepository>().Save(@event.Subject)
            );

            Bus.Register<EntityCreatedEvent<AccountingTransaction>>(
                (@event) =>
                    Container.Resolve<IAccountingTransactionRepository>().Save(@event.Subject)
                );
        }

        private IDomainEventBus Bus
        {
            get { return Container.Resolve<IDomainEventBus>(); }
        }
    }
}
