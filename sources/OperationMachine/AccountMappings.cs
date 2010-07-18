﻿using Meowth.OperationMachine.Domain.Entities.Accounts;
using FluentNHibernate.Mapping;
using Meowth.OperationMachine.Domain.Entities.Transactions;

namespace Meowth.OperationMachine
{
    public class AccountMapping : ClassMap<Account>
    {
        public AccountMapping()
        {
            Id(a => a.Id);
            Map(a => a.Created);
            Map(a => a.CreditTurnover);
            Map(a => a.DebtTurnover);
            Map(a => a.Name);
            References(a => a.Parent);
            Component(
                apn => apn.PathName,
                m => m.Map(n => n.Path));
        }
    }

    public class TransactionMapping : ClassMap<Transaction>
    {
        public TransactionMapping()
        {
            Id(t => t.Id);
            Map(t => t.Amount);
            Map(t => t.Date);
            References(t => t.Destination).Not.Nullable();
            References(t => t.Source).Not.Nullable();
            Map(t => t.IsExecuted);
            Map(t => t.Name);
        }
    }
}