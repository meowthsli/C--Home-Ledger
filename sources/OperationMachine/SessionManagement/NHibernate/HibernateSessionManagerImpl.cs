using System;
using NHibernate;

namespace Meowth.OperationMachine.SessionManagement.NHibernate
{
    public class HibernateSessionManagerImpl : IHibernateSessionManager
    {
        private readonly ISessionFactory _sessionFactory;

        public HibernateSessionManagerImpl(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        [ThreadStatic]
        private static ISession _current;

        public ISession GetActiveSession()
        {
            if (_current == null)
                throw new InvalidOperationException("There is no active ISession instance for this thread");
            return _current;
        }

        public ISession OpenSession()
        {
            if (_current != null)
                throw new InvalidOperationException("There is already an active ISession instance for this thread");

            _current = _sessionFactory.OpenSession();
            return _current;
        }

        public void CloseSession()
        {
            if(_current == null)
                return;

            _current.Flush();
            _current.Close();
            _current.Dispose();
            _current = null;
        }
    }
}
