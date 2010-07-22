using Meowth.Application.CommandParser;
using System;
using System.IO;
using Meowth.OperationMachine.Commands;
using Meowth.OperationMachine.SessionManagement;

namespace Meowth.Application
{
    class Program
    {
        static void Main()
        {
            var om = new OperationMachine.OperationMachine("database.sqlite");
            
            var textOutput = new TextOutput(Console.Out);
            var textParser = new TextCommandParser(Console.In, textOutput);

            using (var uow = om.CreateUnitOfWork())
            {
                var cmd = textParser.ReadCommand();
                while (true)
                {
                    if (cmd == null)
                    {
                        if (textParser.IsQuit())
                        {
                            break;
                        }

                        var errorString = textParser.GetErrorString();
                        var errorPosition = textParser.GetErrorPosition();

                        textOutput.ShowError(errorString, errorPosition);
                    }
                    else
                    {
                        textOutput.ShowDTO(cmd);
                        om.ExecuteCommand((MakeAccountingTransactionCommandDTO) cmd, uow);
                        textOutput.ConfirmOk();
                    }

                    cmd = textParser.ReadCommand();
                }
            }
        }
    }

    public class TextOutput
    {
        private readonly TextWriter _writer;

        public TextOutput(TextWriter writer)
        {
            _writer = writer;
        }

        public void ShowDTO(CommandDTO dto)
        {
            
        }

        public void ConfirmOk()
        {
            _writer.WriteLine(">> Ok");
        }

        public void ShowError(string text, int errorPosition)
        {
            _writer.WriteLine(string.Format(">> Can't parse: '{0}'", text));
        }

        public void Prompt()
        {
            _writer.Write("?> ");
        }
    }
}
