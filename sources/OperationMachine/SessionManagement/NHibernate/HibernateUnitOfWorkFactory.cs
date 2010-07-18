using Meowth.OperationMachine.Domain.DomainInfrastructure;
using NHibernate;

namespace Meowth.OperationMachine.SessionManagement.NHibernate
{
    /// <summary>
    /// Unit of work with real nhibernate actions
    /// </summary>
    public class HibernateUnitOfWorkFactory : IUnitOfWorkFactory
    {
        /// <summary>
        /// .ctor. All deps are in ctor
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <param name="sessionManager"></param>
        /// <param name="eventRouter"></param>
        public HibernateUnitOfWorkFactory(
            ISessionFactory sessionFactory,
            IHibernateSessionManager sessionManager,
            IDomainEventBus eventRouter)
        {
            _sessionFactory = sessionFactory;
            _sessionManager = sessionManager;
            _eventRouter = eventRouter;
        }

        #region IUnitOfWorkFactory Members

        /// <summary>
        /// Creates unit of work to do with
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork CreateUnitOfWork()
        {
            return new HibernateUnitOfWork(_sessionFactory, _sessionManager, _eventRouter);
        }

        #endregion

        private readonly ISessionFactory _sessionFactory;
        private readonly IHibernateSessionManager _sessionManager;
        private readonly IDomainEventBus _eventRouter;

    }
}