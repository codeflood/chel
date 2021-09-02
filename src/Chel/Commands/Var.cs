using System;
using System.Text;
using System.Threading;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;
using Chel.Abstractions.Variables;

namespace Chel
{
    [Command("var")]
    [Description("Manage variables.")]
    internal class Var : ICommand
    {
        private VariableCollection _variables = null;
        private IPhraseDictionary _phraseDictionary = null;
        private INameValidator _nameValidator = null;

        private string _executionCultureName = string.Empty;

        [NumberedParameter(1, "name")]
        [Description("The name of the variable.")]
        public string Name { get; set; }

        [NumberedParameter(2, "value")]
        [Description("The value for the variable.")]
        public ChelType Value { get; set; }

        public Var(VariableCollection variables, IPhraseDictionary phraseDictionary, INameValidator nameValidator)
        {
            if(variables == null)
                throw new ArgumentNullException(nameof(variables));

            if(phraseDictionary == null)
                throw new ArgumentNullException(nameof(phraseDictionary));

            if(nameValidator == null)
                throw new ArgumentNullException(nameof(nameValidator));

            _variables = variables;
            _phraseDictionary = phraseDictionary;
            _nameValidator = nameValidator;
        }

        public CommandResult Execute()
        {
            _executionCultureName = Thread.CurrentThread.CurrentCulture.Name;

            // todo: temporary implementation. Variables should be output as a list once we have those in the type system.
            var output = new StringBuilder();

            if(!string.IsNullOrEmpty(Name) && !_nameValidator.IsValid(Name))
                return new FailureResult(Constants.CurrentSourceLine, new[] { string.Format(Texts.InvalidCharacterInVariableName, Name) });

            if(Value == null)
            {
                if(string.IsNullOrEmpty(Name))
                    ListVariables(output);
                else
                    ShowVariable(output, Name);
            }
            else
            {
                var variable = new Variable(Name, Value);
                _variables.Set(variable);
                output.Append(Value);
            }

            return new ValueResult(output.ToString());
        }

        private void ListVariables(StringBuilder output)
        {
            var names = _variables.Names;
            if(names.Count == 0)
            {
                output.Append(_phraseDictionary.GetPhrase(Texts.PhraseKeys.NoVariablesSet, _executionCultureName));
                return;
            }

            foreach(var name in names)
            {
                var variable = _variables.Get(name);
                output.Append($"{name, Constants.FirstColumnWidth}{variable.Value}{Environment.NewLine}");
            }
        }

        private void ShowVariable(StringBuilder output, string name)
        {
            var variable = _variables.Get(name);
            if(variable == null)
            {
                var template = _phraseDictionary.GetPhrase(Texts.PhraseKeys.VariableNotSet, _executionCultureName);
                output.Append(string.Format(template, name));
                return;
            }

            output.Append(variable.Value);
        }
    }
}