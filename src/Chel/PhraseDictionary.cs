using Chel.Abstractions;
using System;
using System.Collections.Generic;

namespace Chel
{
    public class PhraseDictionary : IPhraseDictionary
    {
        private Dictionary<string, LocalisedTexts> _phrases = new Dictionary<string, LocalisedTexts>();

        public PhraseDictionary()
        {
            var availableCommandsPhrases = new LocalisedTexts();
            availableCommandsPhrases.AddText("Available commands", null);
            _phrases.Add(Texts.PhraseKeys.AvailableCommands, availableCommandsPhrases);

            var missingVariableNamePhrases = new LocalisedTexts();
            missingVariableNamePhrases.AddText("Missing variable name.", null);
            _phrases.Add(Texts.PhraseKeys.MissingVariableName, missingVariableNamePhrases);

            var noVariablesSetPhrases = new LocalisedTexts();
            noVariablesSetPhrases.AddText("No variables set.", null);
            _phrases.Add(Texts.PhraseKeys.NoVariablesSet, noVariablesSetPhrases);

            var requiredPhrases = new LocalisedTexts();
            requiredPhrases.AddText("Required", null);
            _phrases.Add(Texts.PhraseKeys.Required, requiredPhrases);

            var usagePhrases = new LocalisedTexts();
            usagePhrases.AddText("usage", null);
            _phrases.Add(Texts.PhraseKeys.Usage, usagePhrases);

            var variableHasBeenClearedPhrases = new LocalisedTexts();
            variableHasBeenClearedPhrases.AddText("Variable '{0}' has been cleared.", null);
            _phrases.Add(Texts.PhraseKeys.VariableHasBeenCleared, variableHasBeenClearedPhrases);

            var variableNotSetPhrases = new LocalisedTexts();
            variableNotSetPhrases.AddText("Variable '{0}' is not set.", null);
            _phrases.Add(Texts.PhraseKeys.VariableNotSet, variableNotSetPhrases);

            var numberedParameterNotSetPhrases = new LocalisedTexts();
            numberedParameterNotSetPhrases.AddText("Numbered parameter {0} is not set.", null);
            _phrases.Add(Texts.PhraseKeys.NumberedParameterNotSet, numberedParameterNotSetPhrases);

            var couldNotParseNumberPhrases = new LocalisedTexts();
            couldNotParseNumberPhrases.AddText("Could not parse '{0}' as a number.", null);
            _phrases.Add(Texts.PhraseKeys.CouldNotParseNumber, couldNotParseNumberPhrases);
        }

        public string GetPhrase(string phraseKey, string cultureName)
        {
            if(phraseKey == null)
                throw new ArgumentNullException(nameof(phraseKey));

            if(!_phrases.ContainsKey(phraseKey))
                return phraseKey;

            return _phrases[phraseKey].GetText(cultureName);
        }
    }
}