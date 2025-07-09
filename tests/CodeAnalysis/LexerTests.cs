using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;

namespace Pug.Compiler.Tests.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class LexerTests
{
    [Fact]
    public void ExtractTokens_ShouldExtractTokens()
    {
        var lexer = new Lexer("42 true false \"olá mundo\" + - * / = ( ) pow , int x -84.25 if % else end == != > >= < <= && || //ignoro isso");
        var tokens = lexer.ExtractTokens();

        Assert.Equal(29, tokens.Count);

        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(0, tokens[0].Position);
        Assert.Equal("42", tokens[0].Value);

        Assert.Equal(TokenType.Bool, tokens[1].Type);
        Assert.Equal(3, tokens[1].Position);
        Assert.Equal("true", tokens[1].Value);

        Assert.Equal(TokenType.Bool, tokens[2].Type);
        Assert.Equal(8, tokens[2].Position);
        Assert.Equal("false", tokens[2].Value);

        Assert.Equal(TokenType.String, tokens[3].Type);
        Assert.Equal(14, tokens[3].Position);
        Assert.Equal("olá mundo", tokens[3].Value);

        Assert.Equal(TokenType.Plus, tokens[4].Type);
        Assert.Equal(26, tokens[4].Position);
        Assert.Equal("+", tokens[4].Value);

        Assert.Equal(TokenType.Minus, tokens[5].Type);
        Assert.Equal(28, tokens[5].Position);
        Assert.Equal("-", tokens[5].Value);

        Assert.Equal(TokenType.Multiply, tokens[6].Type);
        Assert.Equal(30, tokens[6].Position);
        Assert.Equal("*", tokens[6].Value);

        Assert.Equal(TokenType.Divide, tokens[7].Type);
        Assert.Equal(32, tokens[7].Position);
        Assert.Equal("/", tokens[7].Value);

        Assert.Equal(TokenType.Assign, tokens[8].Type);
        Assert.Equal(34, tokens[8].Position);
        Assert.Equal("=", tokens[8].Value);

        Assert.Equal(TokenType.OpenParenthesis, tokens[9].Type);
        Assert.Equal(36, tokens[9].Position);
        Assert.Equal("(", tokens[9].Value);

        Assert.Equal(TokenType.CloseParenthesis, tokens[10].Type);
        Assert.Equal(38, tokens[10].Position);
        Assert.Equal(")", tokens[10].Value);

        Assert.Equal(TokenType.Function, tokens[11].Type);
        Assert.Equal(40, tokens[11].Position);
        Assert.Equal("pow", tokens[11].Value);

        Assert.Equal(TokenType.Comma, tokens[12].Type);
        Assert.Equal(44, tokens[12].Position);
        Assert.Equal(",", tokens[12].Value);

        Assert.Equal(TokenType.DataType, tokens[13].Type);
        Assert.Equal(46, tokens[13].Position);
        Assert.Equal("int", tokens[13].Value);

        Assert.Equal(TokenType.Identifier, tokens[14].Type);
        Assert.Equal(50, tokens[14].Position);
        Assert.Equal("x", tokens[14].Value);

        Assert.Equal(TokenType.Number, tokens[15].Type);
        Assert.Equal(52, tokens[15].Position);
        Assert.Equal("-84.25", tokens[15].Value);
        
        Assert.Equal(TokenType.If, tokens[16].Type);
        Assert.Equal(59, tokens[16].Position);
        Assert.Equal("if", tokens[16].Value);

        Assert.Equal(TokenType.Remainder, tokens[17].Type);
        Assert.Equal(62, tokens[17].Position);
        Assert.Equal("%", tokens[17].Value);
        
        Assert.Equal(TokenType.Else, tokens[18].Type);
        Assert.Equal(64, tokens[18].Position);
        Assert.Equal("else", tokens[18].Value);
        
        Assert.Equal(TokenType.End, tokens[19].Type);
        Assert.Equal(69, tokens[19].Position);
        Assert.Equal("end", tokens[19].Value);
        
        Assert.Equal(TokenType.Equal, tokens[20].Type);
        Assert.Equal(73, tokens[20].Position);
        Assert.Equal("==", tokens[20].Value);
        
        Assert.Equal(TokenType.NotEqual, tokens[21].Type);
        Assert.Equal(76, tokens[21].Position);
        Assert.Equal("!=", tokens[21].Value);
        
        Assert.Equal(TokenType.Greater, tokens[22].Type);
        Assert.Equal(79, tokens[22].Position);
        Assert.Equal(">", tokens[22].Value);
        
        Assert.Equal(TokenType.GreaterOrEqual, tokens[23].Type);
        Assert.Equal(81, tokens[23].Position);
        Assert.Equal(">=", tokens[23].Value);
        
        Assert.Equal(TokenType.Less, tokens[24].Type);
        Assert.Equal(84, tokens[24].Position);
        Assert.Equal("<", tokens[24].Value);
        
        Assert.Equal(TokenType.LessOrEqual, tokens[25].Type);
        Assert.Equal(86, tokens[25].Position);
        Assert.Equal("<=", tokens[25].Value);
        
        Assert.Equal(TokenType.And, tokens[26].Type);
        Assert.Equal(89, tokens[26].Position);
        Assert.Equal("&&", tokens[26].Value);
        
        Assert.Equal(TokenType.Or, tokens[27].Type);
        Assert.Equal(92, tokens[27].Position);
        Assert.Equal("||", tokens[27].Value);
        
        Assert.Equal(TokenType.EndOfFile, tokens[^1].Type);
        Assert.Equal(108, tokens[^1].Position);
    }

    [Fact]
    public void ExtractTokens_ShouldReturnEOFToken_WhenInputIsEmpty()
    {
        var lexer = new Lexer(string.Empty);
        var tokens = lexer.ExtractTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfFile, tokens[0].Type);
    }

    [Fact]
    public void ExtractTokens_ShouldIgnoreWhitespace()
    {
        var lexer = new Lexer("   ");
        var tokens = lexer.ExtractTokens();

        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfFile, tokens[0].Type);
    }

    [Fact]
    public void ExtractTokens_ShouldThrowException_WhenStringNotClosed()
    {
        var lexer = new Lexer("\"test");

        var exception = Assert.Throws<LexerException>(() => lexer.ExtractTokens());
        Assert.Equal("String not closed", exception.Message);
    }

    [Fact]
    public void ExtractTokens_ShouldThrowException_WhenInvalidNumberFormat()
    {
        var lexer = new Lexer("42..5");

        var exception = Assert.Throws<LexerException>(() => lexer.ExtractTokens());
        Assert.Equal("Invalid number format: multiple dots", exception.Message);
    }

    [Fact]
    public void ExtractTokens_ShouldThrowException_WhenUnexpectedCharacter()
    {
        var lexer = new Lexer("@");

        var exception = Assert.Throws<LexerException>(() => lexer.ExtractTokens());
        Assert.Equal("Unexpected character @", exception.Message);
    }
}