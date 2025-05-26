using System.Text;

namespace Pug.Compiler.CodeAnalysis;

public class Lexer
{
    private readonly string _text;

    private int _currentPosition;
    private char _currentChar;

    private const char EOF = '\0';

    public Lexer(string text)
    {
        _text = text;
        _currentPosition = 0;
        _currentChar = _text.Length > 0 ? _text[_currentPosition] : EOF;
    }

    public List<Token> CreateTokens()
    {
        var tokens = new List<Token>();

        while (_currentChar != EOF)
        {
            if (char.IsWhiteSpace(_currentChar))
            {
                IgnoreWhitespace();
                continue;
            }
            
            if (char.IsLetter(_currentChar))
            {
                tokens.Add(ExtractFunction());
                continue;
            }

            if (_currentChar == '-' && char.IsDigit(Peek()))
            {
                tokens.Add(ExtractNumber());
                continue;
            }

            if (char.IsDigit(_currentChar))
            {
                tokens.Add(ExtractNumber());
                continue;
            }

            var token = _currentChar switch
            {
                '+' => Token.Plus(_currentPosition),
                '-' => Token.Minus(_currentPosition),
                '*' => Token.Multiply(_currentPosition),
                '/' => Token.Divide(_currentPosition),
                '(' => Token.OpenParenthesis(_currentPosition),
                ')' => Token.CloseParenthesis(_currentPosition),
                ',' => Token.Comma(_currentPosition),
                _ => throw new Exception($"Unexpected character: {_currentChar}")
            };
            tokens.Add(token);

            Next();
        }

        tokens.Add(new Token(TokenType.EOF, string.Empty, _currentPosition));
        return tokens;
    }

    private Token ExtractNumber()
    {
        var result = new StringBuilder();

        if (_currentChar == '-')
        {
            result.Append('-');
            Next();
        }

        var hasDot = false;
        while (char.IsDigit(_currentChar) || _currentChar == '.')
        {
            if (_currentChar == '.')
            {
                if (hasDot)
                    throw new Exception("Invalid number format: multiple dots");

                hasDot = true;
            }

            result.Append(_currentChar);
            Next();
        }

        var value = result.ToString();
        return Token.Number(_currentPosition, value);
    }
    
    private Token ExtractFunction()
    {
        var result = new StringBuilder();
        while (char.IsLetter(_currentChar))
        {
            result.Append(_currentChar);
            Next();
        }
        var value = result.ToString();
        return Token.Function(_currentPosition, value);
    }

    private char Peek()
    {
        var peekPos = _currentPosition + 1;
        return peekPos < _text.Length ? _text[peekPos] : EOF;
    }

    private void Next()
    {
        _currentPosition++;
        _currentChar = _currentPosition < _text.Length ? _text[_currentPosition] : EOF;
    }

    private void IgnoreWhitespace()
    {
        while (char.IsWhiteSpace(_currentChar))
            Next();
    }
}