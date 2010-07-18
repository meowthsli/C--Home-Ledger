using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Meowth.OperationMachine.Domain.DomainInfrastructure;
using Meowth.OperationMachine.Domain.Entities.Accounts;
using Meowth.OperationMachine.Domain.Events;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Meowth.OperationMachine.SessionManagement.NHibernate;
using Meowth.OperationMachine.SessionManagement;
using NHibernate;
using Meowth.OperationMachine.Domain.DomainInfrastructure.Repository;
using Meowth.OperationMachine.RepositoryImplementation;
using NHibernate.Tool.hbm2ddl;
using Meowth.OperationMachine.Domain.Entities;
namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    [TestFixture]
    class InMemoryHibernateTests
    {
        private readonly UnityContainer _container = new UnityContainer();
        
        public InMemoryHibernateTests()
        {
            var cfg = Fluently.Configure()
               .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Account>())
               .Database(SQLiteConfiguration.Standard
                   .ConnectionString("Data Source=:memory:;Version=3;BinaryGUID=False;New=True;")
                   .Driver("NHibernate.Driver.SQLite20Driver")
                   .Dialect("NHibernate.Dialect.SQLiteDialect")
                   .QuerySubstitutions("true=1;false=0")
                   .ProxyFactoryFactory("NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu"))
               .BuildConfiguration();

            var sessionFactory = cfg
               .BuildSessionFactory();

            new SchemaExport(cfg).Create(true, true);

            _container
                .RegisterInstance<IDomainEventBus>(new EventRouter())
                .RegisterInstance<ISessionFactory>(sessionFactory)
                .RegisterType<IHibernateSessionManager, HibernateSessionManagerImpl>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IAccountRepository, HibernateAccountRepository>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkFactory, HibernateUnitOfWorkFactory>(
                    new ContainerControlledLifetimeManager());

            var acr = (HibernateAccountRepository)_container.Resolve<IAccountRepository>();
            _container.Resolve<IDomainEventBus>()
                .Register<EntityLifecycleEvent<Account>>(acr.OnAccountCreated);

            DomainEntity.SetEventRouter(_container.Resolve<IDomainEventBus>());
        }

        [Test]
        public void Test()
        {
            
        }
    }
}
