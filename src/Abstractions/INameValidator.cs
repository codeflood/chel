namespace Chel.Abstractions
{
    /// <summary>
    /// Validates names.
    /// </summary>
    public interface INameValidator
    {
        /// <summary>
        /// Indicates whether the name is valid or not.
        /// </summary>
        bool IsValid(string name);
    }
}