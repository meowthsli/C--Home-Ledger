using System;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.SessionManagement;
using System.Diagnostics;

namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    /// <summary>
    /// UoW factory for tests
    /// </summary>
    class TestUoWFactory : IUnitOfWorkFactory
    {
        #region IUnitOfWorkFactory Members

        public IUnitOfWork CreateUnitOfWork()
        {
            return new TestUow();
        }

        #endregion
    }

    /// <summary>
    /// UoW for test
    /// </summary>
    class TestUow : IUnitOfWork
    {
        #region IDisposable Members

        public void Dispose()
        {
        }

        public ITransaction CreateTransaction()
        {
            return new TestTransaction();
        }

        #endregion
    }

    /// <summary>
    /// Transaction wrapper for tests
    /// </summary>
    class TestTransaction : ITransaction
    {
        private bool _committed;
        public TestTransaction()
        {
            Trace.WriteLine("Test transaction created");
        }

        public void Dispose()
        {
            Trace.WriteLine("Test transaction disposed " 
                + (_committed ? "after COMMIT" : "with ROLLBACK"));
        }

        public void Commit()
        {
            _committed = true;
            Trace.WriteLine("Test transaction committed");
        }
    }
}
