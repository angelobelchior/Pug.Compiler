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
        _currentChar = _text.Length > 0 ? _text[_currentPosition] : Token.EOF;
    }

    public List<Token> ExtractTokens()
    {
        var tokens = new List<Token>();

        while (_currentChar != Token.EOF)
        {
            if (char.IsWhiteSpace(_currentChar))
            {
                IgnoreWhitespace();
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

            if (_currentChar == Token.ASSIGN)
            {
                tokens.Add(Token.Assign(_currentPosition));
                Next();
                continue;
            }

            if (_currentChar == Token.MINUS && char.IsDigit(Peek()))
            {
                tokens.Add(ExtractNumber());
                continue;
            }

            if (char.IsDigit(_currentChar))
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

        if (ExpressionResult.ContainsDataType(identifier.Value))
            return Token.DataType(position, identifier.Value);

        if (BuiltInFunctions.Contains(identifier.Value))
            return Token.Function(position, identifier.Value);

        if (identifier.Value is Token.TRUE or Token.FALSE)
            return Token.Bool(position, identifier.Value);

        return Token.Identifier(position, identifier.Value);
    }

    private Token ExtractString()
    {
        var position = _currentPosition;
        
        Next();
        var stringValue = new StringBuilder();

        while (_currentChar != Token.QUOTE && _currentChar != Token.EOF)
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
        var token = _currentChar switch
        {
            Token.PLUS => Token.Plus(position),
            Token.MINUS => Token.Minus(position),
            Token.MULTIPLY => Token.Multiply(position),
            Token.DIVIDER => Token.Divide(_currentPosition),
            Token.OPEN_PARENTHESIS => Token.OpenParenthesis(position),
            Token.CLOSE_PARENTHESIS => Token.CloseParenthesis(position),
            Token.COMMA => Token.Comma(position),
            _ => throw new Exception($"Unexpected character {_currentChar} at position {position}")
        };

        Next();
        return token;
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
        return position < _text.Length ? _text[position] : Token.EOF;
    }

    private void Next()
    {
        _currentPosition++;
        _currentChar = _currentPosition < _text.Length ? _text[_currentPosition] : Token.EOF;
    }

    private void IgnoreWhitespace()
    {
        while (char.IsWhiteSpace(_currentChar))
            Next();
    }
}