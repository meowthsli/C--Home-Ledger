using NHibernate;
using Meowth.OperationMachine.Domain.DomainInfrastructure;

namespace Meowth.OperationMachine.SessionManagement.NHibernate
{
    public class HibernateUnitOfWork : IUnitOfWork
    {
        private readonly IHibernateSessionManager _nhibernateSessionManager;
        private readonly ISession _session;
        private readonly IDomainEventBus _eventRouter;

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

        public void Dispose()
        {
            if (_session == null)
                return;

            _session.Flush();
            _session.Close();
            _session.Dispose();

            _eventRouter.ClearThreaded();
        }
    }
}
