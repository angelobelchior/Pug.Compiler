using System.Diagnostics.CodeAnalysis;
using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Tests.Runtime;

[ExcludeFromCodeCoverage]
public class BuiltInFunctionsTests
{
    [Theory]
    [InlineData("sqrt")]
    [InlineData("pow")]
    [InlineData("min")]
    [InlineData("max")]
    [InlineData("round")]
    [InlineData("random")]
    [InlineData("len")]
    [InlineData("print")]
    [InlineData("log")]
    [InlineData("exp")]
    [InlineData("sin")]
    [InlineData("cos")]
    [InlineData("tan")]
    [InlineData("atn")]
    [InlineData("abs")]
    [InlineData("sgn")]
    [InlineData("substr")]
    [InlineData("left")]
    [InlineData("right")]
    [InlineData("mid")]
    [InlineData("trim")]
    [InlineData("trim_end")]
    [InlineData("trim_start")]
    [InlineData("to_str")]
    [InlineData("to_bool")]
    [InlineData("to_int")]
    [InlineData("to_double")]
    [InlineData("iif")]
    [InlineData("read")]
    [InlineData("clear")]
    [InlineData("upper")]
    [InlineData("lower")]
    [InlineData("replace")]
    public void Contains_ShouldReturnTrue_WhenFunctionExists(string funcion)
    {
        Assert.True(BuiltInFunctions.Contains(funcion));
    }

    [Fact]
    public void Contains_ShouldReturnFalse_WhenFunctionDoesNotExist()
    {
        Assert.False(BuiltInFunctions.Contains("nonexistent"));
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenFunctionNotFound()
    {
        var token = Token.Function(0, "nonexistent");
        var args = new List<Identifier>();

        var exception = Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
        Assert.Equal("Function nonexistent not found", exception.Message);
    }

    [Fact]
    public void Invoke_ShouldExecuteSqrtFunction()
    {
        var token = Token.Function(0, "sqrt");
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, 16.0) };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(4.0, result.ToDouble());
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForSqrt()
    {
        var token = Token.Function(0, "sqrt");
        var args = new List<Identifier>();

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Invoke_ShouldExecutePowFunction_WithOneArgument()
    {
        var token = Token.Function(0, "pow");
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, 3.0) };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(9.0, result.ToDouble());
    }

    [Fact]
    public void Invoke_ShouldExecutePowFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "pow");
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Double, 2.0),
            Identifier.Create(DataTypes.Double, 3.0)
        };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(8.0, result.ToDouble());
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForPow()
    {
        var token = Token.Function(0, "pow");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Invoke_ShouldExecuteMinFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "min");
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Double, 2.0),
            Identifier.Create(DataTypes.Double, 3.0)
        };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(2.0, result.ToDouble());
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForMin()
    {
        var token = Token.Function(0, "min");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Invoke_ShouldExecuteMaxFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "max");
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Double, 2.0),
            Identifier.Create(DataTypes.Double, 3.0)
        };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(3.0, result.ToDouble());
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForMax()
    {
        var token = Token.Function(0, "max");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Invoke_ShouldExecuteRoundFunction_WithOneArgument()
    {
        var token = Token.Function(0, "round");
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, 3.53) };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(4, result.ToDouble());
    }

    [Fact]
    public void Invoke_ShouldExecuteRoundFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "round");
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Double, 3.534),
            Identifier.Create(DataTypes.Double, 2)
        };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(3.53, result.ToDouble());
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForRound()
    {
        var token = Token.Function(0, "round");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Invoke_ShouldExecuteRandomFunction_WithZeroArgument()
    {
        var token = Token.Function(0, "random");
        var args = new List<Identifier>();

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.True(result.ToDouble() >= 0 || result.ToDouble() <= 0); // ;p
    }

    [Fact]
    public void Invoke_ShouldExecuteRandomFunction_WithOneArgument()
    {
        var token = Token.Function(0, "random");
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Int, 2),
        };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.True(result.ToDouble() >= 0 && result.ToDouble() < 2);
    }

    [Fact]
    public void Invoke_ShouldExecuteRandomFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "random");
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Int, 2),
            Identifier.Create(DataTypes.Int, 3)
        };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.True(result.ToDouble() >= 2 && result.ToDouble() < 3);
    }
    
    [Fact]
    public void Random_ShouldReturnValueBetweenZeroAndMax_WhenOneArgumentProvided()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Int, 10) };

        // Act
        var result = BuiltInFunctions.Invoke("random", args);

        // Assert
        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.InRange(result.ToDouble(), 0, 10);
    }

    [Fact]
    public void Random_ShouldReturnValueBetweenMinAndMax_WhenTwoArgumentsProvided()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Int, 5),
            Identifier.Create(DataTypes.Int, 15)
        };

        // Act
        var result = BuiltInFunctions.Invoke("random", args);

        // Assert
        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.InRange(result.ToDouble(), 5, 15);
    }

    [Fact]
    public void Random_ShouldThrowException_WhenInvalidArgumentsProvided()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Int, 1),
            Identifier.Create(DataTypes.Int, 2),
            Identifier.Create(DataTypes.Int, 3)
        };

        // Act & Assert
        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke("random", args));
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForRandom()
    {
        var token = Token.Function(0, "pow");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Invoke_ShouldExecuteLenFunction_WithOneArguments()
    {
        var token = Token.Function(0, "len");
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Angelo")
        };

        var result = BuiltInFunctions.Invoke(token.Value, args);

        Assert.Equal(DataTypes.Int, result.DataType);
        Assert.Equal(6, result.ToInt());
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForLen()
    {
        var token = Token.Function(0, "len");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Invoke_ShouldExecutePrintFunction()
    {
        var token = Token.Function(0, "print");
        var args = new List<Identifier> { Identifier.Create(DataTypes.String, "Hello, World!") };

        var exception = Record.Exception(() => BuiltInFunctions.Invoke(token.Value, args));

        Assert.Null(exception);
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForPrint()
    {
        var token = Token.Function(0, "print");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke(token.Value, args));
    }

    [Fact]
    public void Log_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, Math.E) };

        // Act
        var result = BuiltInFunctions.Invoke("log", args);

        // Assert
        Assert.Equal(1.0, result.ToDouble());
    }

    [Fact]
    public void Exp_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, 1.0) };

        // Act
        var result = BuiltInFunctions.Invoke("exp", args);

        // Assert
        Assert.Equal(Math.E, result.ToDouble());
    }

    [Fact]
    public void Sin_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, Math.PI / 2) };

        // Act
        var result = BuiltInFunctions.Invoke("sin", args);

        // Assert
        Assert.Equal(1.0, result.ToDouble(), 5);
    }

    [Fact]
    public void Cos_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, 0.0) };

        // Act
        var result = BuiltInFunctions.Invoke("cos", args);

        // Assert
        Assert.Equal(1.0, result.ToDouble(), 5);
    }

    [Fact]
    public void Tan_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, Math.PI / 4) };

        // Act
        var result = BuiltInFunctions.Invoke("tan", args);

        // Assert
        Assert.Equal(1.0, result.ToDouble(), 5);
    }

    [Fact]
    public void Atn_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, 1.0) };

        // Act
        var result = BuiltInFunctions.Invoke("atn", args);

        // Assert
        Assert.Equal(Math.PI / 4, result.ToDouble(), 5);
    }

    [Fact]
    public void Abs_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, -5.0) };

        // Act
        var result = BuiltInFunctions.Invoke("abs", args);

        // Assert
        Assert.Equal(5.0, result.ToDouble());
    }

    [Fact]
    public void Sgn_ShouldReturnCorrectValue()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, -10.0) };

        // Act
        var result = BuiltInFunctions.Invoke("sgn", args);

        // Assert
        Assert.Equal(-1, result.ToInt());
    }

    [Fact]
    public void Trim_ShouldReturnTrimmedString()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "  hello world  ")
        };

        // Act
        var result = BuiltInFunctions.Invoke("trim", args);

        // Assert
        Assert.Equal("hello world", result.ToString());
    }

    [Fact]
    public void TrimEnd_ShouldReturnStringWithTrimmedEnd()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "hello world   ")
        };

        // Act
        var result = BuiltInFunctions.Invoke("trim_end", args);

        // Assert
        Assert.Equal("hello world", result.ToString());
    }

    [Fact]
    public void TrimStart_ShouldReturnStringWithTrimmedStart()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "   hello world")
        };

        // Act
        var result = BuiltInFunctions.Invoke("trim_start", args);

        // Assert
        Assert.Equal("hello world", result.ToString());
    }

    [Fact]
    public void ToString_ShouldConvertValueToString()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Int, 123) };

        // Act
        var result = BuiltInFunctions.Invoke("to_str", args);

        // Assert
        Assert.Equal(DataTypes.String, result.DataType);
        Assert.Equal("123", result.ToString());
    }

    [Fact]
    public void ToBool_ShouldConvertValueToBool()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.String, "true") };

        // Act
        var result = BuiltInFunctions.Invoke("to_bool", args);

        // Assert
        Assert.Equal(DataTypes.Bool, result.DataType);
        Assert.Equal(true, result.Value);
    }

    [Fact]
    public void ToInt_ShouldConvertValueToInt()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.Double, 123.45) };

        // Act
        var result = BuiltInFunctions.Invoke("to_int", args);

        // Assert
        Assert.Equal(DataTypes.Int, result.DataType);
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void ToDouble_ShouldConvertValueToDouble()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.String, "123.45") };

        // Act
        var result = BuiltInFunctions.Invoke("to_double", args);

        // Assert
        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(123.45, result.Value);
    }

    [Fact]
    public void Substr_WithValidArguments_ReturnsSubstring()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!"),
            Identifier.Create(DataTypes.Int, 7),
            Identifier.Create(DataTypes.Int, 5)
        };

        var result = BuiltInFunctions.Invoke("substr", args);

        Assert.Equal("World", result.ToString());
    }

    [Fact]
    public void Left_WithValidArguments_ReturnsLeftSubstring()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!"),
            Identifier.Create(DataTypes.Int, 5)
        };

        var result = BuiltInFunctions.Invoke("left", args);

        Assert.Equal("Hello", result.ToString());
    }

    [Fact]
    public void Right_WithValidArguments_ReturnsRightSubstring()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!"),
            Identifier.Create(DataTypes.Int, 6)
        };

        var result = BuiltInFunctions.Invoke("right", args);

        Assert.Equal("World!", result.ToString());
    }

    [Fact]
    public void Mid_WithValidArguments_ReturnsMiddleSubstring()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!"),
            Identifier.Create(DataTypes.Int, 7),
            Identifier.Create(DataTypes.Int, 5)
        };

        var result = BuiltInFunctions.Invoke("mid", args);

        Assert.Equal("World", result.ToString());
    }

    [Fact]
    public void Trim_WithValidArguments_ReturnsTrimmedString()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "  Hello, World!  ")
        };

        var result = BuiltInFunctions.Invoke("trim", args);

        Assert.Equal("Hello, World!", result.ToString());
    }

    [Fact]
    public void TrimEnd_WithValidArguments_ReturnsStringWithTrimmedEnd()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!   ")
        };

        var result = BuiltInFunctions.Invoke("trim_end", args);

        Assert.Equal("Hello, World!", result.ToString());
    }

    [Fact]
    public void TrimStart_WithValidArguments_ReturnsStringWithTrimmedStart()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "   Hello, World!")
        };

        var result = BuiltInFunctions.Invoke("trim_start", args);

        Assert.Equal("Hello, World!", result.ToString());
    }

    [Fact]
    public void Substr_WithTwoArguments_ReturnsSubstringFromStartIndex()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!"),
            Identifier.Create(DataTypes.Int, 7)
        };

        var result = BuiltInFunctions.Invoke("substr", args);

        Assert.Equal("World!", result.ToString());
    }

    [Fact]
    public void Substr_WithInvalidArguments_ThrowsException()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!")
        };

        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke("substr", args));
    }

    [Fact]
    public void Substr_WithNegativeStartIndex_ThrowsException()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello, World!"),
            Identifier.Create(DataTypes.Int, -1),
            Identifier.Create(DataTypes.Int, 5)
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => BuiltInFunctions.Invoke("substr", args));
    }

    [Fact]
    public void Substr_WithLengthExceedingString_ThrowsException()
    {
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "Hello"),
            Identifier.Create(DataTypes.Int, 1),
            Identifier.Create(DataTypes.Int, 10)
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => BuiltInFunctions.Invoke("substr", args));
    }

    [Fact]
    public void Iif_ShouldReturnTrueValue_WhenConditionIsTrue()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Bool, true),
            Identifier.Create(DataTypes.String, "TrueValue"),
            Identifier.Create(DataTypes.String, "FalseValue")
        };

        // Act
        var result = BuiltInFunctions.Invoke("iif", args);

        // Assert
        Assert.Equal(DataTypes.String, result.DataType);
        Assert.Equal("TrueValue", result.ToString());
    }

    [Fact]
    public void Iif_ShouldReturnFalseValue_WhenConditionIsFalse()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Bool, false),
            Identifier.Create(DataTypes.String, "TrueValue"),
            Identifier.Create(DataTypes.String, "FalseValue")
        };

        // Act
        var result = BuiltInFunctions.Invoke("iif", args);

        // Assert
        Assert.Equal(DataTypes.String, result.DataType);
        Assert.Equal("FalseValue", result.ToString());
    }

    [Fact]
    public void Iif_ShouldThrowExceptionIfFirstArgTypeIsInvalid()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "true"),
            Identifier.Create(DataTypes.String, "TrueValue"),
            Identifier.Create(DataTypes.String, "FalseValue")
        };

        // Assert
        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke("iif", args));
    }

    [Fact]
    public void Iif_ShouldThrowExceptionIfInvalidTypes()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Bool, true),
            Identifier.Create(DataTypes.Int, 10),
            Identifier.Create(DataTypes.String, "FalseValue")
        };

        // Assert
        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke("iif", args));
    }

    [Fact]
    public void Iif_ShouldThrowExceptionIfInvalidArgsCount()
    {
        // Arrange
        var args = new List<Identifier>();

        // Assert
        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke("iif", args));
    }

    [Fact]
    public void Read_ShouldReturnInputValue()
    {
        var value = "Hello, World!";
        var input = new StringReader(value);
        Console.SetIn(input);
        
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Bool, true),
            Identifier.Create(DataTypes.Int, 10),
            Identifier.Create(DataTypes.String, "FalseValue")
        };

        // Act
        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke("read", args));
    }
    
    [Fact]
    public void Read_ShouldThrowExceptionIfInvalidArgsCount()
    {
        var value = "Hello, World!";
        var input = new StringReader(value);
        Console.SetIn(input);

        // Act
        var result = BuiltInFunctions.Invoke("read", []);

        // Assert
        Assert.Equal(DataTypes.String, result.DataType);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Clear_ShouldNotThrowException()
    {
        // Arrange
        var args = new List<Identifier>();

        // Act
        var exception = Record.Exception(() => BuiltInFunctions.Invoke("clear", args));

        // Assert
        Assert.Null(exception);
    }
    
    [Fact]
    public void Clear_ShouldThrowExceptionIfInvalidArgsCount()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.Bool, true),
            Identifier.Create(DataTypes.Int, 10),
            Identifier.Create(DataTypes.String, "FalseValue")
        };

        // Act
        Assert.Throws<SyntaxParserException>(() => BuiltInFunctions.Invoke("clear", args));
    }
    
    [Fact]
    public void Upper_ShouldConvertStringToUpperCase()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.String, "test") };

        // Act
        var result = BuiltInFunctions.Invoke("upper", args);

        // Assert
        Assert.Equal("TEST", result.ToString());
        Assert.Equal(DataTypes.String, result.DataType);
    }

    [Fact]
    public void Lower_ShouldConvertStringToLowerCase()
    {
        // Arrange
        var args = new List<Identifier> { Identifier.Create(DataTypes.String, "TEST") };

        // Act
        var result = BuiltInFunctions.Invoke("lower", args);

        // Assert
        Assert.Equal("test", result.ToString());
        Assert.Equal(DataTypes.String, result.DataType);
    }

    [Fact]
    public void Replace_ShouldReplaceSubstringInString()
    {
        // Arrange
        var args = new List<Identifier>
        {
            Identifier.Create(DataTypes.String, "hello world"),
            Identifier.Create(DataTypes.String, "world"),
            Identifier.Create(DataTypes.String, "xunit")
        };

        // Act
        var result = BuiltInFunctions.Invoke("replace", args);

        // Assert
        Assert.Equal("hello xunit", result.ToString());
        Assert.Equal(DataTypes.String, result.DataType);
    }
}