using System;

namespace Chel.Abstractions
{
    /// <summary>
    /// Defines a target to execute.
    /// </summary>
    public struct ExecutionTargetIdentifier
    {
        /// <summary>
        /// Gets the optional module the target belongs to.
        /// </summary>
        public string? Module { get; }

        /// <summary>
        /// Gets the name of the target to execute.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="module">The optional module the target belongs to.</param>
        /// <param name="name">The name of the target to execute.</param>
        public ExecutionTargetIdentifier(string? module, string name)
        {
            Module = module;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString()
        {
            if(Module == null)
                return Name;

            return Module + Symbol.SubName + Name;
        }

        public override bool Equals(object obj)
        {
            if(obj is ExecutionTargetIdentifier other)
            {
                return
                    string.Equals(Module, other.Module, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            var code = 23;
            unchecked
            {
                code += ToString().ToUpperInvariant().GetHashCode();
            }

            return code;
        }
    }
}
