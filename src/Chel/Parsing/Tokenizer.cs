using System;
using System.IO;
using Chel.Abstractions.Parsing;
using Chel.Exceptions;

namespace Chel.Parsing
{
    /// <summary>
    /// Input text processor which emits tokens.
    /// </summary>
	/// <remarks>
	/// The tokenizer is context free. It can emit tokens with local look aheads.
	/// It will handle blocks and comments.
	/// Whitespace in Chel is sometimes significant and other times not. The tokenizer lacks the context to interpret whitespace properly,
	/// so it treats whitespace as literal.
	/// </remarks>
	public class Tokenizer : ITokenizer, IDisposable
	{
		private const char NewLine = '\n';
		private const char Escape = '\\';
		private const char BlockStart = '(';
		private const char BlockEnd = ')';

		private StringReader _reader = null;
		private int _currentLineNumber = 1;
		private int _currentCharacterNumber = 0;
		private bool _escaping = false;

		private SourceLocation CurrentLocation => new SourceLocation(_currentLineNumber, _currentCharacterNumber);

        public Tokenizer(string input)
        {
            if(!string.IsNullOrEmpty(input))
				_reader = new StringReader(input);
        }

		public void Dispose()
		{
			_reader?.Dispose();
		}

		public Token GetNextToken()
		{
			var keepReading = true;

			if(_reader == null)
				keepReading = false;

			while(keepReading)
			{
				keepReading = false;

				var nextChar = _reader.Read();
				_currentCharacterNumber++;

				if(_escaping)
				{
					_escaping = false;
					return HandleEscaped((char)nextChar);
				}

				switch(nextChar)
				{
					case NewLine:
						var result = HandleLiteral((char)nextChar);
						_currentLineNumber++;
						_currentCharacterNumber = 0;
						return result;
					
					case Escape:
						_escaping = true;
						keepReading = true;
						break;

					case BlockStart:
						return HandleSpecial(SpecialTokenType.BlockStart);

					case BlockEnd:
						return HandleSpecial(SpecialTokenType.BlockEnd);

					case -1:
						HandleEndOfStream();
						return null;

					default:
						return HandleLiteral((char)nextChar);
				}
			}

			return null;
		}

		private void HandleEndOfStream()
		{
			_reader.Dispose();
			_reader = null;
		}

		private Token HandleEscaped(char nextChar)
		{
			if(nextChar == BlockStart ||
				nextChar == BlockEnd ||
				nextChar == Escape)
				return HandleLiteral(nextChar);

			throw new ParserException(CurrentLocation, string.Format(Texts.UnknownEscapedCharacter, Escape + nextChar));
		}

		private Token HandleLiteral(char nextChar)
		{
			return new LiteralToken(CurrentLocation, nextChar);
		}

		private Token HandleSpecial(SpecialTokenType type)
		{
			return new SpecialToken(CurrentLocation, type);
		}
	}
}