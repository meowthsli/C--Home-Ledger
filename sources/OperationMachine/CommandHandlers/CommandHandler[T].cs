namespace Meowth.OperationMachine.CommandHandlers
{
    /// <summary> Machine command handlers </summary>
    /// <typeparam name="TDto"></typeparam>
    internal abstract class CommandHandler<TDto>
    {
        /// <summary> Handles command as DTO </summary>
        /// <param name="dto"></param>
        public abstract void Handle(TDto dto);
    }
}
