﻿using System.Diagnostics;
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
using Meowth.OperationMachine.Domain.Entities.Transactions;
using System.IO;
namespace Meowth.OperationMachine.Tests.InfrastructureTests
{
    [TestFixture]
    class InMemoryHibernateTests
    {
        static InMemoryHibernateTests()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private readonly UnityContainer _container = new UnityContainer();
        
        public InMemoryHibernateTests()
        {
            var cfg = Fluently.Configure()
               .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AccountMapping>())
               .Database(SQLiteConfiguration.Standard
                   .ShowSql()
                   .ConnectionString( string.Format("Data Source={0};Version=3;BinaryGUID=True;New=True;Pooling=True;Max Pool Size=1;", Path.GetTempFileName()))
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
                .RegisterInstance(sessionFactory)
                .RegisterType<IHibernateSessionManager, HibernateSessionManagerImpl>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IAccountRepository, HibernateAccountRepository>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IAccountingTransactionRepository, HibernateAccountingTransactionRepository>(
                    new ContainerControlledLifetimeManager())
                .RegisterType<IUnitOfWorkFactory, HibernateUnitOfWorkFactory>(
                    new ContainerControlledLifetimeManager());

            var acr = (HibernateAccountRepository)_container.Resolve<IAccountRepository>();
            _container.Resolve<IDomainEventBus>()
                .Register<EntityCreatedEvent<Account>>(acr.OnAccountCreated);

            var tr = (HibernateAccountingTransactionRepository)_container.Resolve<IAccountingTransactionRepository>();
            _container.Resolve<IDomainEventBus>()
                .Register<EntityCreatedEvent<AccountingTransaction>>(tr.OnTransactionCreated);

            DomainEntity.SetEventRouter(_container.Resolve<IDomainEventBus>());
        }

        [Test]
        public void WhenAccountsCreatedAreCreatedInDatabaseAndTransactionExecutedThenOk()
        {
            OnUow(() =>
                      {
                          var acc1 = new Account("root");
                          var acc2 = new Account("some another");

                          var tx = new AccountingTransaction("tx1", acc1, acc2, 42.0m);
                          //tx.Execute();
                      }
                );
        }

        private void OnUow(Action action)
        {
            using (var uow =_container.Resolve<IUnitOfWorkFactory>().CreateUnitOfWork())
            {
                using (var tx = uow.CreateTransaction())
                {
                    action();
                    tx.Commit();
                }
            }
        }
    }
}
