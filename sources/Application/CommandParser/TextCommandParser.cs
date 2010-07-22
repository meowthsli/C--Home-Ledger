using System;
using System.IO;
using Meowth.OperationMachine.Commands;
using System.Text.RegularExpressions;

namespace Meowth.Application.CommandParser
{
    /// <summary>
    /// Parser for commands in text
    /// </summary>
    public class TextCommandParser
    {
        private readonly TextReader _in;
        private readonly TextOutput _textOutput;
        private string _text = "";

        public TextCommandParser(TextReader @in, TextOutput textOutput)
        {
            _in = @in;
            _textOutput = textOutput;
        }

        /// <summary>
        /// Creates command DTO from text string
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="errorPosition">Position of error in case when return value is null</param>
        /// <returns></returns>
        public CommandDTO ReadCommand()
        {
            _textOutput.Prompt();
            _text = _in.ReadLine();

            var cleanText = _text.Trim().ToLowerInvariant();
            return MatchToTransactionCommand(cleanText);
        }

        private CommandDTO MatchToTransactionCommand(string cleanText)
        {
            if(!_regex.IsMatch(cleanText))
                return null;

            var match = _regex.Match(cleanText);
            if (match == null)
                return null;

            if(match.Groups["from_account"].Value == null
               || match.Groups["to_account"].Value == null
               || match.Groups["amount"].Value == null)
                return null;

            decimal amount;
            if(!decimal.TryParse(match.Groups["amount"].Value, out amount))
                return null;

            return new MakeAccountingTransactionCommandDTO()
                       {
                           Amount = amount,
                           Name = "tx1",
                           DestinationAccountName = match.Groups["to_account"].Value,
                           SourceAccountName = match.Groups["from_account"].Value
                       };
        }

        public string GetErrorString()
        {
            return _text;           
            
        }

        public int GetErrorPosition()
        {
            return 0;
        }

        public bool IsQuit()
        {
            return _text.ToLowerInvariant() == ":quit";
        }

        private Regex _regex = new Regex(
            "^tran(saction)?\\s+"
            + "fr(om)?\\s+"
            + "(?<from_account>[A-Za-z]+)\\s+"
            + "to\\s+"
            + "(?<to_account>[A-Za-z]+)\\s+"
            + "am(ount)?\\s+"
            + "(?<amount>(\\-|\\+)?[0-9]+(\\.)?[0-9]+)\\s*$"
            );
    }
}
