using System;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Entities.Transactions;
using Meowth.OperationMachine.Domain.DomainInfrastructure;

namespace Meowth.OperationMachine.SessionManagement
{
    /// <summary>
    /// Unit of work interface
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Begins business transaction (can be called any times during one unit-of-work)
        /// </summary>
        /// <returns></returns>
        ITransaction CreateTransaction();
    }

    /// <summary>
    /// Transaction wrapper for low-level transaction (i.e. nhibernate)
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Commits transaction
        /// </summary>
        void Commit();
    }

    /// <summary>
    /// Abstract factory for creating backend-dependable unit-of-work
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}
