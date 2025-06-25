using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Tests.Runtime;

[ExcludeFromCodeCoverage]
public class IdentifierTests
{
    [Fact]
    public void Create_ShouldReturnResultWithCorrectTypeAndValue()
    {
        var result = Identifier.Create(DataTypes.Int, 42);

        Assert.Equal(DataTypes.Int, result.DataType);
        Assert.Equal(42, result.Value);
    }

    [Theory]
    [InlineData("42")]
    [InlineData(42)]
    public void Create_WithIntValue_ShouldReturnIntResult(object value)
    {
        var result = Identifier.Create(DataTypes.Int, value);

        Assert.Equal(DataTypes.Int, result.DataType);
        Assert.Equal(42, result.ToInt());
    }

    [Theory]
    [InlineData("Romarinho")]
    [InlineData("")]
    public void Create_WithInvalidValueInt_ShouldThrowException(object value)
    {
        var result = Identifier.Create(DataTypes.Int, value);
        Assert.Throws<Exception>(() => result.ToInt());
    }

    // [Theory]
    // [InlineData("42,5")]
    // [InlineData(42.5)]
    // public void Create_WithDoubleValue_ShouldReturnDoubleResult(object value)
    // {
    //     var result = Identifier.Create(DataTypes.Double, value);
    //
    //     Assert.Equal(DataTypes.Double, result.DataType);
    //     Assert.Equal(42.5, result.AsDouble());
    // }
    
    [Theory]
    [InlineData("Romarinho")]
    [InlineData("")]
    public void Create_WithInvalidValueDouble_ShouldThrowException(object value)
    {
        var result = Identifier.Create(DataTypes.Double, value);
        Assert.Throws<Exception>(() => result.ToDouble());
    }

    [Theory]
    [InlineData("true")]
    [InlineData("True")]
    [InlineData(true)]
    public void Create_WithBoolValueTrue_ShouldReturnBoolResult(object value)
    {
        var result = Identifier.Create(DataTypes.Bool, value);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.True(result.ToBool());
    }
    
    [Theory]
    [InlineData("false")]
    [InlineData("False")]
    [InlineData(false)]
    public void Create_WithBoolValueFalse_ShouldReturnBoolResult(object value)
    {
        var result = Identifier.Create(DataTypes.Bool, value);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.False(result.ToBool());
    }
    
    [Theory]
    [InlineData("Romarinho")]
    [InlineData("")]
    public void Create_WithInvalidValueBoolean_ShouldThrowException(object value)
    {
        var result = Identifier.Create(DataTypes.Bool, value);
        Assert.Throws<Exception>(() => result.ToBool());
    }

    [Fact]
    public void Create_WithStringValue_ShouldReturnStringResult()
    {
        var resul = Identifier.Create(DataTypes.String, "test");

        Assert.Equal(DataTypes.String, resul.DataType);
        Assert.Equal("test", resul.Value);
    }

    [Fact]
    public void FromToken_ShouldReturnResultWithCorrectTypeAndValue()
    {
        var token = Token.Number(1, "42");
        var result = Identifier.FromToken(token);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal("42", result.Value.ToString());
    }

    [Fact]
    public void FromToken_WithNumberToken_ShouldReturnDoubleResult()
    {
        var token = Token.Number(1, "42.5");

        var result = Identifier.FromToken(token);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal("42.5", result.Value);
    }

    [Fact]
    public void FromToken_WithBoolToken_ShouldReturnBoolResult()
    {
        var token = Token.Bool(2, true.ToString());
        var result = Identifier.FromToken(token);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.Equal("True", result.Value);
        
         token = Token.Bool(2, false.ToString());
         result = Identifier.FromToken(token);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.Equal("False", result.Value);
    }

    [Fact]
    public void FromToken_WithStringToken_ShouldReturnStringResult()
    {
        var token = Token.String(3, "test");

        var result = Identifier.FromToken(token);

        Assert.Equal(DataTypes.String, result.DataType);
        Assert.Equal("test", result.Value);
    }

    [Fact]
    public void FromToken_WithInvalidTokenType_ShouldThrowException()
    {
        var token = Token.Identifier(4, "variable");

        var exception = Assert.Throws<Exception>(() => Identifier.FromToken(token));

        Assert.Equal("Invalid token type: Identifier", exception.Message);
    }

    [Fact]
    public void Cast_ShouldConvertResultToSpecifiedType()
    {
        var result = Identifier.Create(DataTypes.Double, 42.5);

        var castedValue = result.Cast("double");

        Assert.Equal(DataTypes.Double, castedValue.DataType);
        Assert.Equal(42.5, castedValue.ToDouble());
    }

    [Fact]
    public void ToString_ShouldReturnCorrectStringRepresentation()
    {
        var result = Identifier.Create(DataTypes.Int, 42);

        Assert.Equal("42", result.Value.ToString());
    }

    [Fact]
    public void Cast_InvalidType_ShouldThrowException()
    {
        var result = Identifier.Create(DataTypes.Int, 42);

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
        var result = Identifier.ContainsDataType(type);

        Assert.Equal(expected, result);
    }
}