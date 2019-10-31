namespace Chel.Abstractions
{
    /// <summary>
    /// Binds parameters to command properties
    /// </summary>
    public interface ICommandParameterBinder
    {
        /// <summary>
        /// Bind the parameters from the input onto the command instance.
        /// </summary>
        /// <returns>The result of the parameter binding.</returns>
        ParameterBindResult Bind(ICommand instance, CommandInput input);
    }
}