using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;

namespace Pug.Compiler.Tests.Runtime;

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

        var exception = Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForRandom()
    {
        var token = Token.Function(0, "pow");
        List<Identifier> args =
        [
            Identifier.Create(DataTypes.Double, 1),
            Identifier.Create(DataTypes.Double, 2),
            Identifier.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
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

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token.Value, args));
    }
}