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

            var noVariablesSetPhrases = new LocalisedTexts();
            noVariablesSetPhrases.AddText("No variables set.", null);
            _phrases.Add(Texts.PhraseKeys.NoVariablesSet, noVariablesSetPhrases);

            var requiredPhrases = new LocalisedTexts();
            requiredPhrases.AddText("Required", null);
            _phrases.Add(Texts.PhraseKeys.Required, requiredPhrases);

            var usagePhrases = new LocalisedTexts();
            usagePhrases.AddText("usage", null);
            _phrases.Add(Texts.PhraseKeys.Usage, usagePhrases);

            var variableNotSetPhrases = new LocalisedTexts();
            variableNotSetPhrases.AddText("Variable '{0}' is not set.", null);
            _phrases.Add(Texts.PhraseKeys.VariableNotSet, variableNotSetPhrases);
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