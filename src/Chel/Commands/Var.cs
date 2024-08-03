using System;
using System.Collections.Generic;
using System.Threading;
using Chel.Abstractions;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;

namespace Chel.Commands
{
    [Command("var")]
    [Description("Manage variables.")]
    internal class Var : ICommand
    {
        private readonly VariableCollection _variables;
        private readonly INameValidator _nameValidator;

        private string _executionCultureName = string.Empty;

        [NumberedParameter(1, "name")]
        [Description("The name of the variable.")]
        public string? Name { get; set; }

        [NumberedParameter(2, "value")]
        [Description("The value for the variable.")]
        public ChelType? Value { get; set; }

        [FlagParameter("clear")]
        [Description("Clear the variable.")]
        public bool Clear { get; set; }

        public Var(VariableCollection variables, INameValidator nameValidator)
        {
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _nameValidator = nameValidator ?? throw new ArgumentNullException(nameof(nameValidator));
        }

        public CommandResult Execute()
        {
            _executionCultureName = Thread.CurrentThread.CurrentCulture.Name;

            ChelType? output = null;

            if(!string.IsNullOrEmpty(Name) && !_nameValidator.IsValid(Name))
                return new FailureResult(
                    ApplicationTextResolver.Instance.ResolveAndFormat(ApplicationTexts.InvalidCharacterInVariableName, Name ?? string.Empty)
                );

            if(Clear && string.IsNullOrEmpty(Name))
                return new FailureResult(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.MissingVariableName));

            if(Value == null)
            {
                if(string.IsNullOrEmpty(Name))
                    output = ListVariables();
                else
                    output = ShowOrClearVariable();
            }
            else
            {
                var variable = new Variable(Name!, Value);
                _variables.Set(variable);
                output = Value;
            }

            if(output == null)
                return SuccessResult.Instance;

            return new ValueResult(output);
        }

        private ChelType ListVariables()
        {
            var names = _variables.Names;
            if(names.Count == 0)
                return new Literal(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.NoVariablesSet));

            var output = new Dictionary<string, ICommandParameter>();

            foreach(var name in names)
            {
                var variable = _variables.Get(name);
                output.Add(variable!.Name, variable.Value);
            }

            return new Map(output);
        }

        private ChelType? ShowOrClearVariable()
        {
            if(Name == null)
                return null;

            var variable = _variables.Get(Name);
            if(variable == null)
            {
                var template = ApplicationTextResolver.Instance.Resolve(ApplicationTexts.VariableNotSet);
                return new Literal(string.Format(template, Name));
            }

            if(Clear)
            {
                _variables.Remove(Name);
                return new Literal(string.Format(ApplicationTextResolver.Instance.Resolve(ApplicationTexts.VariableHasBeenCleared), Name));
            }

            return variable.Value;
        }
    }
}