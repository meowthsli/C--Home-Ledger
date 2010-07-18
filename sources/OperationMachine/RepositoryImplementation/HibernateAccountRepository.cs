using System;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Events;
using Meowth.OperationMachine.SessionManagement.NHibernate;
using NHibernate;

namespace Meowth.OperationMachine.RepositoryImplementation
{
    /// <summary>
    /// Accounts repository with real nhibernate backend
    /// </summary>
    public class HibernateAccountRepository : IAccountRepository
    {
        private readonly IHibernateSessionManager _sessionManager;

        #region IAccountRepository Members

        public HibernateAccountRepository(IHibernateSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public Account FindByPathName(AccountPathName name)
        {
            throw new NotImplementedException();
        }

        public Account GetRootAccount()
        {
            throw new NotImplementedException();
        }

        public void OnAccountCreated(EntityCreatedEvent<Account> accountCreatedEvent)
        {
            GetActiveSession().Save(accountCreatedEvent.Subject);
        }

        #endregion

        private ISession GetActiveSession()
        {
            return _sessionManager.GetActiveSession();
        }
    }
}
