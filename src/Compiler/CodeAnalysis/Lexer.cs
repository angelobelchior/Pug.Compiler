using System.Text;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.CodeAnalysis;

public class Lexer
{
    private readonly string _text;

    private int _currentPosition;
    private char _currentChar;

    public Lexer(string text)
    {
        _text = text;
        _currentPosition = 0;
        _currentChar = _text.Length > 0 ? _text[_currentPosition] : Token.END_OF_FILE;
    }

    public List<Token> ExtractTokens()
    {
        var tokens = new List<Token>();

        while (_currentChar != Token.END_OF_FILE)
        {
            if (char.IsWhiteSpace(_currentChar))
            {
                IgnoreWhitespace();
                continue;
            }

            if (_currentChar == Token.DIVIDER && Peek() == Token.DIVIDER)
            {
                IgnoreComment();
                continue;
            }

            if (char.IsLetter(_currentChar))
            {
                tokens.Add(ExtractKeyword());
                continue;
            }

            if (_currentChar == Token.QUOTE)
            {
                tokens.Add(ExtractString());
                continue;
            }

            if (_currentChar == Token.EQUAL && Peek() != Token.EQUAL)
            {
                tokens.Add(Token.Assign(_currentPosition));
                Next();
                continue;
            }

            if (_currentChar == Token.MINUS && char.IsDigit(Peek()) || char.IsDigit(_currentChar))
            {
                tokens.Add(ExtractNumber());
                continue;
            }

            tokens.Add(ExtractSymbols());
        }

        tokens.Add(Token.EndOfFile(_currentPosition));
        return tokens;
    }

    private Token ExtractKeyword()
    {
        var position = _currentPosition;
        var identifier = ExtractIdentifier();

        if (Identifier.ContainsDataType(identifier.Value))
            return Token.DataType(position, identifier.Value);

        if (BuiltInFunctions.Contains(identifier.Value))
            return Token.Function(position, identifier.Value);

        if (identifier.Value is Token.TRUE or Token.FALSE)
            return Token.Bool(position, identifier.Value);

        if (identifier.Value == Token.IF)
            return Token.If(position);

        if (identifier.Value == Token.THEN)
            return Token.Then(position);

        if (identifier.Value == Token.ELSE)
            return Token.Else(position);

        if (identifier.Value == Token.END)
            return Token.End(position);

        return Token.Identifier(position, identifier.Value);
    }

    private Token ExtractString()
    {
        var position = _currentPosition;

        Next();
        var stringValue = new StringBuilder();

        while (_currentChar != Token.QUOTE && _currentChar != Token.END_OF_FILE)
        {
            stringValue.Append(_currentChar);
            Next();
        }

        if (_currentChar != Token.QUOTE)
            throw new Exception("String not closed");

        Next();
        return Token.String(position, stringValue.ToString());
    }

    private Token ExtractSymbols()
    {
        var position = _currentPosition;

        var doubleCharToken = (_currentChar, Peek()) switch
        {
            (Token.AMPERSAND, Token.AMPERSAND) => Token.And(position),
            (Token.PIPE, Token.PIPE) => Token.Or(position),
            (Token.EQUAL, Token.EQUAL) => Token.Equal(position),
            (Token.NOT, Token.EQUAL) => Token.NotEqual(position),
            (Token.GREATER, Token.EQUAL) => Token.GreaterOrEqual(position),
            (Token.LESS, Token.EQUAL) => Token.LessOrEqual(position),
            _ => null
        };
        
        if (doubleCharToken is not null)
        {
            Next(2);
            return doubleCharToken;
        }

        var singleCharTokenFactory = new Dictionary<char, Func<int, Token>>
        {
            [Token.PLUS] = Token.Plus,
            [Token.MINUS] = Token.Minus,
            [Token.MULTIPLY] = Token.Multiply,
            [Token.DIVIDER] = Token.Divide,
            [Token.OPEN_PARENTHESIS] = Token.OpenParenthesis,
            [Token.CLOSE_PARENTHESIS] = Token.CloseParenthesis,
            [Token.COMMA] = Token.Comma,
            [Token.GREATER] = Token.Greater,
            [Token.LESS] = Token.Less
        };

        if (singleCharTokenFactory.TryGetValue(_currentChar, out var tokenFactory))
        {
            Next();
            return tokenFactory(position);
        }

        throw new Exception($"Unexpected character {_currentChar}");
    }

    private Token ExtractNumber()
    {
        var position = _currentPosition;
        var number = new StringBuilder();
        var hasDot = false;

        if (_currentChar == Token.MINUS)
        {
            number.Append(Token.MINUS);
            Next();
        }

        while (char.IsDigit(_currentChar) || _currentChar == Token.DOT)
        {
            if (_currentChar == Token.DOT)
            {
                if (hasDot)
                    throw new Exception("Invalid number format: multiple dots");

                hasDot = true;
            }

            number.Append(_currentChar);
            Next();
        }

        if (hasDot && !double.TryParse(number.ToString(), out _))
            throw new Exception($"Invalid number format: {number}");

        return Token.Number(position, number.ToString());
    }

    private Token ExtractIdentifier()
    {
        var position = _currentPosition;
        var result = new StringBuilder();

        while (char.IsLetter(_currentChar))
        {
            result.Append(_currentChar);
            Next();
        }

        return Token.Identifier(position, result.ToString());
    }

    private char Peek()
    {
        var position = _currentPosition + 1;
        return position < _text.Length ? _text[position] : Token.END_OF_FILE;
    }

    private void Next(int count = 1)
    {
        _currentPosition += count;
        _currentChar = _currentPosition < _text.Length ? _text[_currentPosition] : Token.END_OF_FILE;
    }

    private void IgnoreWhitespace()
    {
        while (char.IsWhiteSpace(_currentChar))
            Next();
    }

    private void IgnoreComment()
    {
        while (_currentChar != Token.NEW_LINE && _currentChar != Token.END_OF_FILE)
            Next();

        if (_currentChar == Token.NEW_LINE)
            Next();
    }
}