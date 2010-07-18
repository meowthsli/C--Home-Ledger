using Meowth.OperationMachine.Domain.Accounts;
using Meowth.OperationMachine.Domain.Entities.Accounts;

namespace Meowth.OperationMachine.Domain.DomainInfrastructure.Repository
{
    public interface IAccountRepository
    {
        Account FindByPathName(AccountPathName name);

        Account GetRootAccount();
    }
}
