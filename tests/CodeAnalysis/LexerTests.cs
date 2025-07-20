using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Tests.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class LexerTests
{
    [Theory]
    [InlineData("", TokenType.EndOfFile, Token.END_OF_FILE, 0)]
    [InlineData("               ", TokenType.EndOfFile, Token.END_OF_FILE, 15)]
    [InlineData("123", TokenType.Number, "123", 0)]
    [InlineData("-123", TokenType.Number, "-123", 0)]
    [InlineData("12.3", TokenType.Number, "12.3", 0)]
    [InlineData("-12.3", TokenType.Number, "-12.3", 0)]
    [InlineData("true", TokenType.Bool, "true", 0)]
    [InlineData("false", TokenType.Bool, "false", 0)]
    [InlineData("\"this is a string\"", TokenType.String, "this is a string", 0)]
    [InlineData("+", TokenType.Plus, "+", 0)]
    [InlineData("-", TokenType.Minus, "-", 0)]
    [InlineData("*", TokenType.Multiply, "*", 0)]
    [InlineData("/", TokenType.Divide, "/", 0)]
    [InlineData("=", TokenType.Assign, "=", 0)]
    [InlineData("(", TokenType.OpenParenthesis, "(", 0)]
    [InlineData(")", TokenType.CloseParenthesis, ")", 0)]
    [InlineData("pow(2)", TokenType.Function, "pow", 0)]
    [InlineData(",", TokenType.Comma, ",", 0)]
    [InlineData("my_variable", TokenType.Identifier, "my_variable", 0)]
    [InlineData("int", TokenType.DataType, "int", 0)]
    [InlineData("string", TokenType.DataType, "string", 0)]
    [InlineData("bool", TokenType.DataType, "bool", 0)]
    [InlineData("double", TokenType.DataType, "double", 0)]
    [InlineData("if", TokenType.If, "if", 0)]
    [InlineData("else", TokenType.Else, "else", 0)]
    [InlineData("end", TokenType.End, "end", 0)]
    [InlineData("==", TokenType.Equal, "==", 0)]
    [InlineData("!=", TokenType.NotEqual, "!=", 0)]
    [InlineData(">", TokenType.Greater, ">", 0)]
    [InlineData(">=", TokenType.GreaterOrEqual, ">=", 0)]
    [InlineData("<", TokenType.Less, "<", 0)]
    [InlineData("<=", TokenType.LessOrEqual, "<=", 0)]
    [InlineData("&&", TokenType.And, "&&", 0)]
    [InlineData("||", TokenType.Or, "||", 0)]
    [InlineData("// nothing here...", TokenType.EndOfFile, Token.END_OF_FILE, 18)]
    [InlineData("""
                
                // nothing here...
                
                """, TokenType.EndOfFile, Token.END_OF_FILE, 20)]
    public void Lexer_ShouldExtractToken(
        string input,
        TokenType tokenType,
        object value,
        int position)
    {
        // Arrange
        var lexer = new Lexer(input);

        // Act
        var tokens = lexer.ExtractTokens();

        // Assert
        Assert.NotEmpty(tokens);
        var token = tokens.FirstOrDefault();
        Assert.NotNull(token);
        Assert.Equal(tokenType, token.Type);
        Assert.Equal(position, token.Position);
        Assert.Equal(value.ToString(), token.Value);
    }
    
    [Fact]
    public void Lexer_ShouldThrowException_WhenStringNotClosed()
    {
        // Arrange
        var input = "\"this is a string";
        var lexer = new Lexer(input);

        // Act & Assert
        var exception = Assert.Throws<LexerException>(() => lexer.ExtractTokens());
        Assert.Equal("String not closed", exception.Message);
    }
    
    [Fact]
    public void Lexer_ShouldThrowException_WhenInvalidNumber()
    {
        // Arrange
        var input = "12.34.56";
        var lexer = new Lexer(input);

        // Act & Assert
        var exception = Assert.Throws<LexerException>(() => lexer.ExtractTokens());
        Assert.Equal("Invalid number format: multiple dots", exception.Message);
    }
    
    [Fact]
    public void Lexer_ShouldThrowException_WhenUnexpectedCharacter()
    {
        // Arrange
        var input = "?";
        var lexer = new Lexer(input);

        // Act & Assert
        var exception = Assert.Throws<LexerException>(() => lexer.ExtractTokens());
        Assert.Equal("Unexpected character ?", exception.Message);
    }
}