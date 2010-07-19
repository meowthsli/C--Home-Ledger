using System;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Transactions;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.SessionManagement.NHibernate;

namespace Meowth.OperationMachine.RepositoryImplementation
{
    public sealed class HibernateAccountingTransactionRepository : IAccountingTransactionRepository
    {
        private readonly IHibernateSessionManager _sessionManager;

        /// <summary>
        /// .Ctor with all dependencies
        /// </summary>
        /// <param name="sessionManager"></param>
        public HibernateAccountingTransactionRepository(IHibernateSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        #region IAccountingTransactionRepository Members

        /// <summary>
        /// Returns transaction by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AccountingTransaction GetById(Guid id)
        {
            return _sessionManager.GetActiveSession().Get<AccountingTransaction>(id);
        }

        #endregion

        public void OnTransactionCreated(EntityCreatedEvent<AccountingTransaction> tx)
        {
            _sessionManager.GetActiveSession().Save(tx.Subject);
        }
    }
}
