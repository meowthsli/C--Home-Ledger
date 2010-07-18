using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.SessionManagement;

namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    class TestUoWFactory : IUnitOfWorkFactory
    {
        #region IUnitOfWorkFactory Members

        public IUnitOfWork CreateUnitOfWork()
        {
            return new Uow();
        }

        class Uow : IUnitOfWork
        {
            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion
        }
        #endregion
    }
}
