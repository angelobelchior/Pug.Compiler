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
        Assert.Throws<SyntaxParserException>(() => result.ToInt());
    }

    [Theory]
    [InlineData("Romarinho")]
    [InlineData("")]
    public void Create_WithInvalidValueDouble_ShouldThrowException(object value)
    {
        var result = Identifier.Create(DataTypes.Double, value);
        Assert.Throws<SyntaxParserException>(() => result.ToDouble());
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
        Assert.Throws<SyntaxParserException>(() => result.ToBool());
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
        Assert.Equal(42.5, result.Value);
    }

    [Fact]
    public void FromToken_WithBoolToken_ShouldReturnBoolResult()
    {
        var token = Token.Bool(2, true.ToString());
        var result = Identifier.FromToken(token);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.Equal(true, result.Value);
        
         token = Token.Bool(2, false.ToString());
         result = Identifier.FromToken(token);

        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.Equal(false, result.Value);
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

        var exception = Assert.Throws<SyntaxParserException>(() => Identifier.FromToken(token));

        Assert.Equal("Invalid token type: Identifier", exception.Message);
    }

    [Fact]
    public void Cast_ShouldConvertResultToSpecifiedType()
    {
        var result = Identifier.Create(DataTypes.Double, 42.5);

        var castedValue = result.Cast("double");

        Assert.Equal(DataTypes.Double, castedValue.DataType);
        Assert.Equal(42.5, castedValue.Value);
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

        Assert.Throws<SyntaxParserException>(() => result.Cast("unknown"));
    }
    
    [Theory]
    [InlineData("int", DataTypes.Int, 0)]
    [InlineData("double", DataTypes.Double, 0.0)]
    [InlineData("bool", DataTypes.Bool, false)]
    [InlineData("string", DataTypes.String, "")]
    public void Default_ShouldReturnCorrectIdentifier(string typeName, DataTypes expectedType, object expectedValue)
    {
        // Act
        var result = Identifier.Default(typeName);

        // Assert
        Assert.Equal(expectedType, result.DataType);
        Assert.Equal(expectedValue, result.Value);
    }
  
    [Fact]
    public void Default_ShouldThrowExceptionForInvalidType()
    {
        // Arrange
        var invalidType = "invalid";

        // Act & Assert
        var exception = Assert.Throws<SyntaxParserException>(() => Identifier.Default(invalidType));
        Assert.Equal($"Invalid data type: {invalidType}", exception.Message);
    }
    
    [Fact]
    public void Value_ShouldReturnInt_WhenDataTypeIsInt()
    {
        // Arrange
        var identifier = new Identifier(DataTypes.Int, 42);

        // Act
        var value = identifier.Value;

        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void Value_ShouldReturnDouble_WhenDataTypeIsDouble()
    {
        // Arrange
        var identifier = new Identifier(DataTypes.Double, 42.5);

        // Act
        var value = identifier.Value;

        // Assert
        Assert.Equal(42.5, value);
    }

    [Fact]
    public void Value_ShouldReturnBoolAsString_WhenDataTypeIsBool()
    {
        // Arrange
        var identifier = new Identifier(DataTypes.Bool, true);

        // Act
        var value = identifier.Value;

        // Assert
        Assert.Equal(true, value);
    }

    [Fact]
    public void Value_ShouldReturnString_WhenDataTypeIsString()
    {
        // Arrange
        var identifier = new Identifier(DataTypes.String, "test");

        // Act
        var value = identifier.Value;

        // Assert
        Assert.Equal("test", value);
    }

    [Fact]
    public void Value_ShouldReturnNone_WhenDataTypeIsNone()
    {
        // Arrange
        var identifier = new Identifier(DataTypes.None, None.Value);

        // Act
        var value = identifier.Value;

        // Assert
        Assert.Equal(Identifier.None, value);
    }
    
    [Fact]
    public void Value_ShouldThrowException_WhenDataTypeIsInvalid()
    {
        // Arrange
        var identifier = new Identifier((DataTypes)9999, None.Value);

        Assert.Throws<SyntaxParserException>(() =>
        {
            _ = identifier.Value;
        });
    }
}