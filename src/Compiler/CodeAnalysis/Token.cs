namespace Pug.Compiler.CodeAnalysis;

public class Token
{
    public const char END_OF_FILE = '\0';
    public const char NEW_LINE = '\n';

    public const char QUOTE = '"';

    public const char COMMA = ',';
    public const char DOT = '.';

    public const char PLUS = '+';
    public const char MINUS = '-';
    public const char MULTIPLY = '*';
    public const char DIVIDER = '/';
    public const char REMAINDER = '%';
    public const char EQUAL = '=';

    public const char OPEN_PARENTHESIS = '(';
    public const char CLOSE_PARENTHESIS = ')';

    public const string TRUE = "true";
    public const string FALSE = "false";

    public const string IF = "if";
    public const string ELSE = "else";
    public const string END = "end";
    public const char GREATER = '>';
    public const char LESS = '<';
    public const char NOT = '!';
    public const char AMPERSAND = '&';
    public const char PIPE = '|';

    public TokenType Type { get; }
    public string Value { get; }
    public int Position { get; }

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
    
    public static Token Remainder(int position)
        => new(TokenType.Remainder, REMAINDER, position);

    public static Token Assign(int position)
        => new(TokenType.Assign, EQUAL, position);

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

    public static Token If(int position)
        => new(TokenType.If, IF, position);
    
    public static Token Else(int position)
        => new(TokenType.Else, ELSE, position);

    public static Token End(int position)
        => new(TokenType.End, END, position);

    public static Token Greater(int position)
        => new(TokenType.Greater, GREATER, position);

    public static Token Less(int position)
        => new(TokenType.Less, LESS, position);

    public static Token GreaterOrEqual(int position)
        => new(TokenType.GreaterOrEqual, GREATER + EQUAL.ToString(), position);

    public static Token LessOrEqual(int position)
        => new(TokenType.LessOrEqual, LESS + EQUAL.ToString(), position);

    public static Token Equal(int position)
        => new(TokenType.Equal, EQUAL + EQUAL.ToString(), position);

    public static Token NotEqual(int position)
        => new(TokenType.NotEqual, NOT + EQUAL.ToString(), position);

    public static Token And(int position)
        => new(TokenType.And, AMPERSAND + AMPERSAND.ToString(), position);

    public static Token Or(int position)
        => new(TokenType.Or, PIPE + PIPE.ToString(), position);


    public static bool IsMathOperatorType(TokenType type)
        => type is
            TokenType.Equal or
            TokenType.NotEqual or
            TokenType.Greater or
            TokenType.GreaterOrEqual or
            TokenType.Less or
            TokenType.LessOrEqual;

    public override string ToString()
        => $"TokenType.{Type} => {Value} (Position: {Position})";
}