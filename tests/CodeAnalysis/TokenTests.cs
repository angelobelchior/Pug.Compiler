using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Tests.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class TokenTests
{
    [Fact]
    public void EndOfFile_ShouldCreateCorrectToken()
    {
        var position = 0;
        var token = Token.EndOfFile(position);

        Assert.Equal(TokenType.EndOfFile, token.Type);
        Assert.Equal(Token.END_OF_FILE.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Plus_ShouldCreateCorrectToken()
    {
        var position = 1;
        var token = Token.Plus(position);

        Assert.Equal(TokenType.Plus, token.Type);
        Assert.Equal(Token.PLUS.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void GreaterOrEqual_ShouldCreateCorrectToken()
    {
        var position = 2;
        var token = Token.GreaterOrEqual(position);

        Assert.Equal(TokenType.GreaterOrEqual, token.Type);
        Assert.Equal(Token.GREATER + Token.EQUAL.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Identifier_ShouldCreateCorrectToken()
    {
        var position = 3;
        var value = "myVar";
        var token = Token.Identifier(position, value);

        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal(value, token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void IsMathOperatorType_ShouldReturnTrueForMathOperators()
    {
        Assert.True(Token.IsMathOperatorType(TokenType.Equal));
        Assert.True(Token.IsMathOperatorType(TokenType.NotEqual));
        Assert.True(Token.IsMathOperatorType(TokenType.Greater));
        Assert.True(Token.IsMathOperatorType(TokenType.GreaterOrEqual));
        Assert.True(Token.IsMathOperatorType(TokenType.Less));
        Assert.True(Token.IsMathOperatorType(TokenType.LessOrEqual));
    }

    [Fact]
    public void IsMathOperatorType_ShouldReturnFalseForNonMathOperators()
    {
        Assert.False(Token.IsMathOperatorType(TokenType.Plus));
        Assert.False(Token.IsMathOperatorType(TokenType.Minus));
        Assert.False(Token.IsMathOperatorType(TokenType.Identifier));
    }

    [Fact]
    public void Minus_ShouldCreateCorrectToken()
    {
        var position = 4;
        var token = Token.Minus(position);

        Assert.Equal(TokenType.Minus, token.Type);
        Assert.Equal(Token.MINUS.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Multiply_ShouldCreateCorrectToken()
    {
        var position = 5;
        var token = Token.Multiply(position);

        Assert.Equal(TokenType.Multiply, token.Type);
        Assert.Equal(Token.MULTIPLY.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Divide_ShouldCreateCorrectToken()
    {
        var position = 6;
        var token = Token.Divide(position);

        Assert.Equal(TokenType.Divide, token.Type);
        Assert.Equal(Token.DIVIDER.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Remainder_ShouldCreateCorrectToken()
    {
        var position = 7;
        var token = Token.Remainder(position);

        Assert.Equal(TokenType.Remainder, token.Type);
        Assert.Equal(Token.REMAINDER.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Assign_ShouldCreateCorrectToken()
    {
        var position = 8;
        var token = Token.Assign(position);

        Assert.Equal(TokenType.Assign, token.Type);
        Assert.Equal(Token.EQUAL.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void OpenParenthesis_ShouldCreateCorrectToken()
    {
        var position = 9;
        var token = Token.OpenParenthesis(position);

        Assert.Equal(TokenType.OpenParenthesis, token.Type);
        Assert.Equal(Token.OPEN_PARENTHESIS.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void CloseParenthesis_ShouldCreateCorrectToken()
    {
        var position = 10;
        var token = Token.CloseParenthesis(position);

        Assert.Equal(TokenType.CloseParenthesis, token.Type);
        Assert.Equal(Token.CLOSE_PARENTHESIS.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Comma_ShouldCreateCorrectToken()
    {
        var position = 11;
        var token = Token.Comma(position);

        Assert.Equal(TokenType.Comma, token.Type);
        Assert.Equal(Token.COMMA.ToString(), token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Function_ShouldCreateCorrectToken()
    {
        var position = 12;
        var value = "myFunction";
        var token = Token.Function(position, value);

        Assert.Equal(TokenType.Function, token.Type);
        Assert.Equal(value, token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void Bool_ShouldCreateCorrectToken()
    {
        var position = 13;
        var value = Token.TRUE;
        var token = Token.Bool(position, value);

        Assert.Equal(TokenType.Bool, token.Type);
        Assert.Equal(value, token.Value);
        Assert.Equal(position, token.Position);
    }

    [Fact]
    public void String_ShouldCreateCorrectToken()
    {
        var position = 14;
        var value = "myString";
        var token = Token.String(position, value);

        Assert.Equal(TokenType.String, token.Type);
        Assert.Equal(value, token.Value);
        Assert.Equal(position, token.Position);
    }
    
    [Fact]
    public void ToString_ShouldReturnCorrectFormat()
    {
        var position = 15;
        var value = "example";
        var token = Token.Identifier(position, value);

        var result = token.ToString();

        Assert.Equal($"TokenType.{TokenType.Identifier} => {value} (Position: {position})", result);
    }
}