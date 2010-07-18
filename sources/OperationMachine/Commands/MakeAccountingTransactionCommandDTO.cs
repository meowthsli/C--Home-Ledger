using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meowth.OperationMachine.Commands
{
    public class MakeAccountingTransactionCommandDTO
    {
        public string Name { get; set; }
        public string SourceAccountName { get; set; }
        public string DestinationAccountName { get; set; }
        public decimal Amount { get; set; }
    }
}
