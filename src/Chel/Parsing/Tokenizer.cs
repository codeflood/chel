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
		private const char Comment = '#';

		private StringReader _reader = null;
		private int _currentLineNumber = 1;
		private int _currentCharacterNumber = 0;

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
			if(_reader == null)
				return null;

			var nextChar = ReadNext();

			switch(nextChar)
			{
				case NewLine:
					return HandleNewLine();
				
				case Escape:
					return HandleEscaped();

				case Comment:
					if(PeekNext() == BlockEnd)
						throw new ParseException(CurrentLocation, Texts.MissingCommentBlockStart);

					return SkipToEndOfLine();

				case BlockStart:
					if(PeekNext() == Comment)
					{
						ReadNext();
						SkipToEndOfCommentBlock();
						return GetNextToken();
					}

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

		private int ReadNext()
		{
			_currentCharacterNumber++;
			return _reader.Read();
		}

		private int PeekNext()
		{
			return _reader.Peek();
		}

		private Token SkipToEndOfLine()
		{
			var nextChar = ReadNext();

			while(nextChar != -1 && nextChar != NewLine)
			{
				nextChar = ReadNext();
			}

			if(nextChar == -1)
				return null;

			return HandleNewLine();
		}

		private void SkipToEndOfCommentBlock()
		{
			var currentLocation = CurrentLocation;
			var startLocation = new SourceLocation(currentLocation.LineNumber, currentLocation.CharacterNumber - 1);

			var nextChar = ReadNext();
			var prevChar = nextChar;

			while(nextChar != -1 && prevChar != Comment && nextChar != BlockEnd)
			{
				prevChar = nextChar;
				nextChar = ReadNext();
			}

			if(nextChar == -1)
				throw new ParseException(startLocation, Texts.MissingCommentBlockEnd);
		}

		private void HandleEndOfStream()
		{
			_reader.Dispose();
			_reader = null;
		}

		private Token HandleNewLine()
		{
			var result = HandleLiteral(NewLine);		
			_currentLineNumber++;
			_currentCharacterNumber = 0;
			return result;
		}

		private Token HandleEscaped()
		{
			var nextChar = ReadNext();
			if(nextChar == -1)
				return null;

			if(nextChar == BlockStart ||
				nextChar == BlockEnd ||
				nextChar == Escape ||
				nextChar == Comment)
				return HandleLiteral((char)nextChar);

			// Report the error location as the escape character
			var currentLocation = CurrentLocation;
			var location = new SourceLocation(currentLocation.LineNumber, currentLocation.CharacterNumber - 1);
			throw new ParseException(location, string.Format(Texts.UnknownEscapedCharacter, (char)nextChar));
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