using System.Diagnostics;
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
using System;
namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    [TestFixture]
    class InMemoryHibernateTests
    {
        private readonly UnityContainer _container = new UnityContainer();
        
        public InMemoryHibernateTests()
        {
            var cfg = Fluently.Configure()
               .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AccountMapping>())
               .Database(SQLiteConfiguration.Standard
                   .ConnectionString("Data Source=:memory:;Version=3;BinaryGUID=True;New=True;Pooling=True;Max Pool Size=1;")
                   .Driver("NHibernate.Driver.SQLite20Driver")
                   .Dialect("NHibernate.Dialect.SQLiteDialect")
                   .QuerySubstitutions("true=1;false=0")
                   .ProxyFactoryFactory("NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu"))
               .BuildConfiguration();

            var sessionFactory = cfg
               .BuildSessionFactory();

            foreach (var md in sessionFactory.GetAllClassMetadata())
                Trace.WriteLine(md.Key);

            foreach (var md in sessionFactory.GetAllCollectionMetadata())
                Trace.WriteLine(md.Key);

            new SchemaExport(cfg).Execute(true, true, false);

            _container
                .RegisterInstance<IDomainEventBus>(new DomainEventBus())
                .RegisterInstance<ISessionFactory>(sessionFactory)
                .RegisterType<IHibernateSessionManager, HibernateSessionManagerImpl>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IAccountRepository, HibernateAccountRepository>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkFactory, HibernateUnitOfWorkFactory>(
                    new ContainerControlledLifetimeManager());

            var acr = (HibernateAccountRepository)_container.Resolve<IAccountRepository>();
            _container.Resolve<IDomainEventBus>()
                .Register<EntityCreatedEvent<Account>>(acr.OnAccountCreated);

            DomainEntity.SetEventRouter(_container.Resolve<IDomainEventBus>());
        }

        [Test]
        public void Test()
        {
            OnUow(() =>
                      {
                          var acc = new Account("root");
                      }
                );
        }

        private void OnUow(Action action)
        {
            using (_container.Resolve<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                action();
            }
        }
    }
}
