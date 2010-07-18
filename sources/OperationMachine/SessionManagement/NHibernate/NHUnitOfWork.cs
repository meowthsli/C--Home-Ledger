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
        private readonly ISession _session;
        private readonly IDomainEventBus _eventRouter;

        /// <summary>
        /// .ctor with all dependencies
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="nhibernateSessionManager"></param>
        /// <param name="eventRouter"></param>
        public HibernateUnitOfWork(
            ISessionFactory sessionFactory, 
            IHibernateSessionManager nhibernateSessionManager,
            IDomainEventBus eventRouter)
        {
            _nhibernateSessionManager = nhibernateSessionManager;
            _session = sessionFactory.OpenSession();
            _nhibernateSessionManager.SetActiveSession(_session);

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
