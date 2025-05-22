namespace Pug.Compiler.CodeAnalysis;

public class Token
{
    public const char END_OF_FILE = '\0';

    public const char QUOTE = '"';

    public const char COMMA = ',';
    public const char DOT = '.';

    public const char PLUS = '+';
    public const char MINUS = '-';
    public const char MULTIPLY = '*';
    public const char DIVIDER = '/';
    public const char ASSIGN = '=';

    public const char OPEN_PARENTHESIS = '(';
    public const char CLOSE_PARENTHESIS = ')';

    public const string TRUE = "true";
    public const string FALSE = "false";

    public TokenType Type { get; }
    public string Value { get; }
    public int Position { get; }

    public static Token EndOfFile(int position)
        => new(TokenType.EndOfFile, END_OF_FILE, position);

    public static Token DataType(int position, string value)
        => new(TokenType.DataType, value, position);

    public static Token Number(int position, string value)
        => new(TokenType.Number, value, position);

    public static Token Bool(int position, string value)
        => new(TokenType.Bool, value, position);

    public static Token String(int position, string value)
        => new(TokenType.String, value, position);

    public static Token Plus(int position)
        => new(TokenType.Plus, PLUS, position);

    public static Token Minus(int position)
        => new(TokenType.Minus, MINUS, position);

    public static Token Multiply(int position)
        => new(TokenType.Multiply, MULTIPLY, position);

    public static Token Divide(int position)
        => new(TokenType.Divide, DIVIDER, position);

    public static Token Assign(int position)
        => new(TokenType.Assign, ASSIGN, position);

    public static Token OpenParenthesis(int position)
        => new(TokenType.OpenParenthesis, OPEN_PARENTHESIS, position);

    public static Token CloseParenthesis(int position)
        => new(TokenType.CloseParenthesis, CLOSE_PARENTHESIS, position);

    public static Token Function(int position, string value)
        => new(TokenType.Function, value, position);

    public static Token Comma(int position)
        => new(TokenType.Comma, COMMA.ToString(), position);

    public static Token Identifier(int position, string value)
        => new(TokenType.Identifier, value, position);

    private Token(TokenType tokenType, string value, int position)
    {
        Type = tokenType;
        Value = value;
        Position = position;
    }

    private Token(TokenType tokenType, char value, int position)
        : this(tokenType, value.ToString(), position)
    {
    }

    public override string ToString()
        => $"TokenType.{Type} => {Value} (Position: {Position})";
}