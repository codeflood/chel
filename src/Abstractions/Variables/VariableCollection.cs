using System;
using System.Collections.Generic;
using System.Linq;

namespace Chel.Abstractions.Variables
{
    /// <summary>
    /// A collection of variables.
    /// </summary>
    /// <remark>This class is internal to prevent custom commands using it directly.</remark>
    internal class VariableCollection
    {
        private Dictionary<string, Variable> _variables;

        /// <summary>
        /// Gets the names of all the variables in the collection.
        /// </summary>
        public IList<string> Names => _variables.Select(x => x.Key).ToList();

        public VariableCollection()
        {
            _variables = new Dictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sets a variable in the collection.
        /// </summary>
        /// <param name="variable">The variable to set.</param>
        public void Set(Variable variable)
        {
            if(variable == null)
                throw new ArgumentNullException(nameof(variable));

            if(_variables.ContainsKey(variable.Name))
                _variables[variable.Name] = variable;
            else
                _variables.Add(variable.Name, variable);
        }

        /// <summary>
        /// Remove a variable from the collection.
        /// </summary>
        public void Remove(string name)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrEmpty(name))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeNullOrEmpty, nameof(name), nameof(name));

            if(_variables.ContainsKey(name))
                _variables.Remove(name);
        }

        /// <summary>
        /// Gets a variable from the collection.
        /// </summary>
        public Variable? Get(string name)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(string.IsNullOrEmpty(name))
                throw ExceptionFactory.CreateArgumentException(ApplicationTexts.ArgumentCannotBeNullOrEmpty, nameof(name), nameof(name));

            if(_variables.ContainsKey(name))
                return _variables[name];

            return null;
        }
    }
}