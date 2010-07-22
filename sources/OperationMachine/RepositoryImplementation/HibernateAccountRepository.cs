using System;
using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using System.Linq;
using Meowth.OperationMachine.SessionManagement.NHibernate;
using NHibernate;
using NHibernate.Linq;

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
            var fullName = AccountPathName.FromParentNameAndString(GetRootAccount().PathName, name.Path);
            var res = GetActiveSession().Linq<Account>()
                .Where(a => a.PathName == fullName)
                .Take(1).ToArray();
            
            return res.Length == 0 ? null : res[0];
        }

        public Account GetRootAccount()
        {
            var res = GetActiveSession().Linq<Account>()
                .Where(a => a.Level == 0)
                .Take(1).ToArray();

            return res.Length == 0? null : res[0];
        }

        public void Save(Account account)
        {
            GetActiveSession().Save(account);
        }

        #endregion

        private ISession GetActiveSession()
        {
            return _sessionManager.GetActiveSession();
        }
    }
}
