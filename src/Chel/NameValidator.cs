using Chel.Abstractions;

namespace Chel
{
    public class NameValidator : INameValidator
    {
        public bool IsValid(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                return false;

            if(name.StartsWith("-"))
                return false;

            return true;
        }
    }
}