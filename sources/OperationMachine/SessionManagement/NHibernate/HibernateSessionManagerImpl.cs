using System;
using NHibernate;

namespace Meowth.OperationMachine.SessionManagement.NHibernate
{
    public class HibernateSessionManagerImpl : IHibernateSessionManager
    {
        [ThreadStatic]
        private static ISession _current;

        public ISession GetActiveSession()
        {
            if (_current == null)
                throw new InvalidOperationException("There is no active ISession instance for this thread");
            return _current;
        }

        public void SetActiveSession(ISession session)
        {
            if (_current != null)
                throw new InvalidOperationException("There is already an active ISession instance for this thread");

            _current = session;
        }

        public void ClearActiveSession()
        {
            _current = null;
        }
    }
}
