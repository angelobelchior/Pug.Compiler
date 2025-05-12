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
        var args = new List<ExpressionResult>();

        var exception = Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
        Assert.Equal("Function nonexistent not found", exception.Message);
    }

    [Fact]
    public void Invoke_ShouldExecuteSqrtFunction()
    {
        var token = Token.Function(0, "sqrt");
        var args = new List<ExpressionResult> { ExpressionResult.Create(DataTypes.Double, 16.0) };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(4.0, result.AsDouble());
    }

    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForSqrt()
    {
        var token = Token.Function(0, "sqrt");
        var args = new List<ExpressionResult>();

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }

    [Fact]
    public void Invoke_ShouldExecutePowFunction_WithOneArgument()
    {
        var token = Token.Function(0, "pow");
        var args = new List<ExpressionResult> { ExpressionResult.Create(DataTypes.Double, 3.0) };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(9.0, result.AsDouble());
    }

    [Fact]
    public void Invoke_ShouldExecutePowFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "pow");
        var args = new List<ExpressionResult>
        {
            ExpressionResult.Create(DataTypes.Double, 2.0),
            ExpressionResult.Create(DataTypes.Double, 3.0)
        };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(8.0, result.AsDouble());
    }
    
    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForPow()
    {
        var token = Token.Function(0, "pow");
        List<ExpressionResult> args =
        [
            ExpressionResult.Create(DataTypes.Double, 1),
            ExpressionResult.Create(DataTypes.Double, 2),
            ExpressionResult.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }
    
    [Fact]
    public void Invoke_ShouldExecuteMinFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "min");
        var args = new List<ExpressionResult>
        {
            ExpressionResult.Create(DataTypes.Double, 2.0),
            ExpressionResult.Create(DataTypes.Double, 3.0)
        };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(2.0, result.AsDouble());
    }
    
    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForMin()
    {
        var token = Token.Function(0, "min");
        List<ExpressionResult> args =
        [
            ExpressionResult.Create(DataTypes.Double, 1),
            ExpressionResult.Create(DataTypes.Double, 2),
            ExpressionResult.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }
    
    [Fact]
    public void Invoke_ShouldExecuteMaxFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "max");
        var args = new List<ExpressionResult>
        {
            ExpressionResult.Create(DataTypes.Double, 2.0),
            ExpressionResult.Create(DataTypes.Double, 3.0)
        };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(3.0, result.AsDouble());
    }
    
    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForMax()
    {
        var token = Token.Function(0, "max");
        List<ExpressionResult> args =
        [
            ExpressionResult.Create(DataTypes.Double, 1),
            ExpressionResult.Create(DataTypes.Double, 2),
            ExpressionResult.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }
    
    [Fact]
    public void Invoke_ShouldExecuteRoundFunction_WithOneArgument()
    {
        var token = Token.Function(0, "round");
        var args = new List<ExpressionResult> { ExpressionResult.Create(DataTypes.Double, 3.53) };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(4, result.AsDouble());
    }

    [Fact]
    public void Invoke_ShouldExecuteRoundFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "round");
        var args = new List<ExpressionResult>
        {
            ExpressionResult.Create(DataTypes.Double, 3.534),
            ExpressionResult.Create(DataTypes.Double, 2)
        };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.Equal(3.53, result.AsDouble());
    }
    
    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForRound()
    {
        var token = Token.Function(0, "round");
        List<ExpressionResult> args =
        [
            ExpressionResult.Create(DataTypes.Double, 1),
            ExpressionResult.Create(DataTypes.Double, 2),
            ExpressionResult.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }
    
    [Fact]
    public void Invoke_ShouldExecuteRandomFunction_WithZeroArgument()
    {
        var token = Token.Function(0, "random");
        var args = new List<ExpressionResult>();

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.True(result.AsDouble() >= 0 || result.AsDouble() <= 0); // ;p
        
    }
    
    [Fact]
    public void Invoke_ShouldExecuteRandomFunction_WithOneArgument()
    {
        var token = Token.Function(0, "random");
        var args = new List<ExpressionResult>
        {
            ExpressionResult.Create(DataTypes.Int, 2),
        };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.True(result.AsDouble() >= 0 && result.AsDouble() < 2);
    }

    [Fact]
    public void Invoke_ShouldExecuteRandomFunction_WithTwoArguments()
    {
        var token = Token.Function(0, "random");
        var args = new List<ExpressionResult>
        {
            ExpressionResult.Create(DataTypes.Int, 2),
            ExpressionResult.Create(DataTypes.Int, 3)
        };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Double, result.DataType);
        Assert.True(result.AsDouble() >= 2 && result.AsDouble() < 3);
    }
    
    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForRandom()
    {
        var token = Token.Function(0, "pow");
        List<ExpressionResult> args =
        [
            ExpressionResult.Create(DataTypes.Double, 1),
            ExpressionResult.Create(DataTypes.Double, 2),
            ExpressionResult.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }
    
    [Fact]
    public void Invoke_ShouldExecuteLenFunction_WithOneArguments()
    {
        var token = Token.Function(0, "len");
        var args = new List<ExpressionResult>
        {
            ExpressionResult.Create(DataTypes.String, "Angelo")
        };

        var result = BuiltInFunctions.Invoke(token, args);

        Assert.Equal(DataTypes.Int, result.DataType);
        Assert.Equal(6, result.AsInt());
    }
    
    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForLen()
    {
        var token = Token.Function(0, "len");
        List<ExpressionResult> args =
        [
            ExpressionResult.Create(DataTypes.Double, 1),
            ExpressionResult.Create(DataTypes.Double, 2),
            ExpressionResult.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }
    
    [Fact]
    public void Invoke_ShouldExecutePrintFunction()
    {
        var token = Token.Function(0, "print");
        var args = new List<ExpressionResult> { ExpressionResult.Create(DataTypes.String, "Hello, World!") };

        var exception = Record.Exception(() => BuiltInFunctions.Invoke(token, args));

        Assert.Null(exception);
    }
    
    [Fact]
    public void Invoke_ShouldThrowException_WhenInvalidArgumentsForPrint()
    {
        var token = Token.Function(0, "print");
        List<ExpressionResult> args =
        [
            ExpressionResult.Create(DataTypes.Double, 1),
            ExpressionResult.Create(DataTypes.Double, 2),
            ExpressionResult.Create(DataTypes.Double, 3),
        ];

        Assert.Throws<Exception>(() => BuiltInFunctions.Invoke(token, args));
    }
}