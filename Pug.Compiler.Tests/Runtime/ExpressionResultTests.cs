using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Tests.Runtime;

[ExcludeFromCodeCoverage]
public class ExpressionResultTests
{
    [Fact]
    public void Create_ShouldReturnResultWithCorrectTypeAndValue()
    {
        var result = ExpressionResult.Create(DataTypes.Int, 42);

        Assert.Equal(DataTypes.Int, result.DataType);
        Assert.Equal(42, result.Value);
    }

    [Theory]
    [InlineData("42")]
    [InlineData(42)]
    public void Create_WithIntValue_ShouldReturnIntResult(object value)
    {
        var result = ExpressionResult.Create(DataTypes.Int, value);

        Assert.Equal(DataTypes.Int, result.DataType);
        Assert.Equal(42, result.AsInt());
    }

    [Theory]
    [InlineData("Romarinho")]
    [InlineData("")]
    public void Create_WithInvalidValueInt_ShouldThrowException(object value)
    {
        var result = ExpressionResult.Create(DataTypes.Int, value);
        Assert.Throws<Exception>(() => result.AsInt());
    }

    [Theory]
    [InlineData("42,5")]
    [InlineData(42.5)]
    public void Create_WithDoubleValue_ShouldReturnDoubleResult(object value)
    {
        var result = ExpressionResult.Create(DataTypes.Double, value);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(42.5, result.AsDouble());
    }
    
    [Theory]
    [InlineData("Romarinho")]
    [InlineData("")]
    public void Create_WithInvalidValueDouble_ShouldThrowException(object value)
    {
        var result = ExpressionResult.Create(DataTypes.Double, value);
        Assert.Throws<Exception>(() => result.AsDouble());
    }

    [Theory]
    [InlineData("true")]
    [InlineData("True")]
    [InlineData(true)]
    public void Create_WithBoolValueTrue_ShouldReturnBoolResult(object value)
    {
        var result = ExpressionResult.Create(DataTypes.Bool, value);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.True(result.AsBool());
    }
    
    [Theory]
    [InlineData("false")]
    [InlineData("False")]
    [InlineData(false)]
    public void Create_WithBoolValueFalse_ShouldReturnBoolResulte(object value)
    {
        var result = ExpressionResult.Create(DataTypes.Bool, value);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.False(result.AsBool());
    }
    
    [Theory]
    [InlineData("Romarinho")]
    [InlineData("")]
    public void Create_WithInvalidValueBoolean_ShouldThrowException(object value)
    {
        var result = ExpressionResult.Create(DataTypes.Bool, value);
        Assert.Throws<Exception>(() => result.AsBool());
    }

    [Fact]
    public void Create_WithStringValue_ShouldReturnStringResult()
    {
        var resul = ExpressionResult.Create(DataTypes.String, "test");

        Assert.Equal(DataTypes.String, resul.DataType);
        Assert.Equal("test", resul.Value);
    }

    [Fact]
    public void Create_WithoutParameters_ShouldReturnVoidResult()
    {
        var result = ExpressionResult.Create();

        Assert.Equal(DataTypes.Empty, result.DataType);
        Assert.Equal(ExpressionResultVoid.Void, result.Value);
    }

    [Fact]
    public void FromToken_ShouldReturnResultWithCorrectTypeAndValue()
    {
        var token = Token.Number(1, "42");
        var result = ExpressionResult.FromToken(token);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal("42", result.Value.ToString());
    }

    [Fact]
    public void FromToken_WithNumberToken_ShouldReturnDoubleResult()
    {
        var token = Token.Number(1, "42.5");

        var result = ExpressionResult.FromToken(token);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal("42.5", result.Value);
    }

    [Fact]
    public void FromToken_WithBoolToken_ShouldReturnBoolResult()
    {
        var token = Token.Bool(2, "true");

        var result = ExpressionResult.FromToken(token);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.Equal("true", result.Value);
    }

    [Fact]
    public void FromToken_WithStringToken_ShouldReturnStringResult()
    {
        var token = Token.String(3, "test");

        var result = ExpressionResult.FromToken(token);

        Assert.Equal(DataTypes.String, result.DataType);
        Assert.Equal("test", result.Value);
    }

    [Fact]
    public void FromToken_WithInvalidTokenType_ShouldThrowException()
    {
        var token = Token.Identifier(4, "variable");

        var exception = Assert.Throws<Exception>(() => ExpressionResult.FromToken(token));

        Assert.Equal("Invalid token type: Identifier", exception.Message);
    }

    [Fact]
    public void Cast_ShouldConvertResultToSpecifiedType()
    {
        var result = ExpressionResult.Create(DataTypes.Double, 42.5);

        var castedValue = result.Cast("int");

        Assert.Equal(DataTypes.Int, castedValue.DataType);
        Assert.Equal(42, castedValue.Value);
    }

    [Fact]
    public void ToString_ShouldReturnCorrectStringRepresentation()
    {
        var result = ExpressionResult.Create(DataTypes.Int, 42);

        Assert.Equal("42", result.Value.ToString());
    }

    [Fact]
    public void Cast_InvalidType_ShouldThrowException()
    {
        var result = ExpressionResult.Create(DataTypes.Int, 42);

        Assert.Throws<Exception>(() => result.Cast("unknown"));
    }
    
    [Theory]
    [InlineData("int", true)]
    [InlineData("double", true)]
    [InlineData("bool", true)]
    [InlineData("string", true)]
    [InlineData("unknown", false)]
    [InlineData("", false)]
    public void ContainDataType_ShouldReturnExpectedResult(string type, bool expected)
    {
        var result = ExpressionResult.ContainsDataType(type);

        Assert.Equal(expected, result);
    }
}