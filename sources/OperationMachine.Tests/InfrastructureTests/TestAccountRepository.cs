using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.Domain.Entities.Accounts;

namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    class TestAccountRepository : IAccountRepository
    {
        private readonly Account _root;
        private Account _subroot1;
        private Account _subroot2;

        #region IAccountRepository Members

        public TestAccountRepository()
        {
            _root = new Account("root");
            _subroot1 = _root.CreateSubaccount("subroot1");
            _subroot2 = _root.CreateSubaccount("subroot2");
        }

        public Account FindByPathName(AccountPathName name)
        {
            if (name.Equals(_subroot1.PathName))
                return _subroot1;

            if (name.Equals(_subroot2.PathName))
                return _subroot2;

            return _root;
        }

        public Account GetRootAccount()
        {
            return _root;
        }

        #endregion
    }
}
