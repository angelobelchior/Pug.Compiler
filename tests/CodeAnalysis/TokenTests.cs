using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Tests.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class TokenTests
{
    [Fact]
    public void EndOfFile_ShouldCreateEOFToken()
    {
        var token = Token.EndOfFile(0);

        Assert.Equal(TokenType.EndOfFile, token.Type);
        Assert.Equal("\0", token.Value);
        Assert.Equal(0, token.Position);
    }

    [Fact]
    public void DataType_ShouldCreateDataTypeToken()
    {
        var token = Token.DataType(1, "int");

        Assert.Equal(TokenType.DataType, token.Type);
        Assert.Equal("int", token.Value);
        Assert.Equal(1, token.Position);
    }

    [Fact]
    public void Number_ShouldCreateIntToken()
    {
        var token = Token.Number(2, "42");

        Assert.Equal(TokenType.Number, token.Type);
        Assert.Equal("42", token.Value);
        Assert.Equal(2, token.Position);
    }
    
    [Fact]
    public void Number_ShouldCreateDoubleToken()
    {
        var token = Token.Number(2, "42.22");

        Assert.Equal(TokenType.Number, token.Type);
        Assert.Equal("42.22", token.Value);
        Assert.Equal(2, token.Position);
    }

    [Fact]
    public void Bool_ShouldCreateBoolToken()
    {
        var token = Token.Bool(3, "true");

        Assert.Equal(TokenType.Bool, token.Type);
        Assert.Equal("true", token.Value);
        Assert.Equal(3, token.Position);
    }

    [Fact]
    public void String_ShouldCreateStringToken()
    {
        var token = Token.String(4, "test");

        Assert.Equal(TokenType.String, token.Type);
        Assert.Equal("test", token.Value);
        Assert.Equal(4, token.Position);
    }

    [Fact]
    public void Plus_ShouldCreatePlusToken()
    {
        var token = Token.Plus(5);

        Assert.Equal(TokenType.Plus, token.Type);
        Assert.Equal("+", token.Value);
        Assert.Equal(5, token.Position);
    }

    [Fact]
    public void Assign_ShouldCreateAssignToken()
    {
        var token = Token.Assign(6);

        Assert.Equal(TokenType.Assign, token.Type);
        Assert.Equal("=", token.Value);
        Assert.Equal(6, token.Position);
    }

    [Fact]
    public void Identifier_ShouldCreateIdentifierToken()
    {
        var token = Token.Identifier(7, "variable");

        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal("variable", token.Value);
        Assert.Equal(7, token.Position);
    }

    [Fact]
    public void Minus_ShouldCreateMinusToken()
    {
        var token = Token.Minus(8);

        Assert.Equal(TokenType.Minus, token.Type);
        Assert.Equal("-", token.Value);
        Assert.Equal(8, token.Position);
    }

    [Fact]
    public void Multiply_ShouldCreateMultiplyToken()
    {
        var token = Token.Multiply(9);

        Assert.Equal(TokenType.Multiply, token.Type);
        Assert.Equal("*", token.Value);
        Assert.Equal(9, token.Position);
    }

    [Fact]
    public void Divide_ShouldCreateDivideToken()
    {
        var token = Token.Divide(10);

        Assert.Equal(TokenType.Divide, token.Type);
        Assert.Equal("/", token.Value);
        Assert.Equal(10, token.Position);
    }

    [Fact]
    public void OpenParenthesis_ShouldCreateOpenParenthesisToken()
    {
        var token = Token.OpenParenthesis(11);

        Assert.Equal(TokenType.OpenParenthesis, token.Type);
        Assert.Equal("(", token.Value);
        Assert.Equal(11, token.Position);
    }

    [Fact]
    public void CloseParenthesis_ShouldCreateCloseParenthesisToken()
    {
        var token = Token.CloseParenthesis(12);

        Assert.Equal(TokenType.CloseParenthesis, token.Type);
        Assert.Equal(")", token.Value);
        Assert.Equal(12, token.Position);
    }

    [Fact]
    public void Function_ShouldCreateFunctionToken()
    {
        var token = Token.Function(13, "myFunction");

        Assert.Equal(TokenType.Function, token.Type);
        Assert.Equal("myFunction", token.Value);
        Assert.Equal(13, token.Position);
    }

    [Fact]
    public void Comma_ShouldCreateCommaToken()
    {
        var token = Token.Comma(14);

        Assert.Equal(TokenType.Comma, token.Type);
        Assert.Equal(",", token.Value);
        Assert.Equal(14, token.Position);
    }
}