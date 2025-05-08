namespace Pug.Compiler.CodeAnalysis;

public record Token(TokenType Type, string Value, int Position)
{
    public static Token EOF(int position) => new(TokenType.EOF, string.Empty, position);

    public static Token Number(int position, string value) => new(TokenType.Number, value, position);
    
    public static Token Plus(int position) => new(TokenType.Plus, "+", position);
    public static Token Minus(int position) => new(TokenType.Minus, "-", position);
    public static Token Multiply(int position) => new(TokenType.Multiply, "*", position);
    public static Token Divide(int position) => new(TokenType.Divide, "/", position);
    
    public static Token OpenParenthesis(int position) => new(TokenType.OpenParenthesis, "(", position);
    public static Token CloseParenthesis(int position) => new(TokenType.CloseParenthesis, ")", position);
    
    public static Token Function(int position, string value) => new(TokenType.Function, value, position);
    public static Token Comma(int position) => new(TokenType.Comma, ",", position);

}