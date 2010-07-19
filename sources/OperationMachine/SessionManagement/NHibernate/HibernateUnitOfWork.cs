using System;
using NHibernate;
using Meowth.OperationMachine.Domain.DomainInfrastructure;

namespace Meowth.OperationMachine.SessionManagement.NHibernate
{
    /// <summary>
    /// Unit of work for nhibernate sessions
    /// </summary>
    public class HibernateUnitOfWork : IUnitOfWork
    {
        private readonly IHibernateSessionManager _nhibernateSessionManager;
        private readonly IDomainEventBus _eventRouter;
        private ISession _session;

        /// <summary>
        /// .ctor with all dependencies
        /// </summary>
        /// <param name="nhibernateSessionManager"></param>
        /// <param name="eventRouter"></param>
        public HibernateUnitOfWork(
            IHibernateSessionManager nhibernateSessionManager,
            IDomainEventBus eventRouter)
        {
            _nhibernateSessionManager = nhibernateSessionManager;
            _session = _nhibernateSessionManager.OpenSession();

            _eventRouter = eventRouter;
        }

        /// <summary> Killer </summary>
        public void Dispose()
        {
            if (_session == null)
                return;

            _session.Flush();
            _session.Close();
            _session.Dispose();

            _eventRouter.ClearThreadedSubscribers();

            _session = null;
        }

        /// <summary> Creates new wrapper for hibernate transaction </summary>
        /// <returns></returns>
        public ITransaction CreateTransaction()
        {
            return new HibernateTransaction(_session.BeginTransaction());
        }
    }

    /// <summary> Transaction wrapper for nhibernate </summary>
    class HibernateTransaction : ITransaction
    {
        /// <summary>
        /// Simple .ctor
        /// </summary>
        /// <param name="tx"></param>
        public HibernateTransaction(global::NHibernate.ITransaction tx) { _transaction = tx; }
        
        #region ITransaction Members

        /// <summary>
        /// Commit underlaying transaction
        /// </summary>
        public void Commit()
        {
            if(_commited)
                throw new InvalidOperationException();

            _transaction.Commit(); // exception
            _commited = true;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// End transaction
        /// </summary>
        public void Dispose()
        {
            _transaction.Dispose();
        }

        #endregion

        /// <summary>
        /// Is already commited
        /// </summary>
        private bool _commited;

        /// <summary>
        /// NHibernate transaction
        /// </summary>
        private readonly global::NHibernate.ITransaction _transaction;
    }
}
