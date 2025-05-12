using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Pug.Compiler.Tests;

[TestCaseOrderer("Pug.Compiler.Tests.InlineDataOrderer", "Pug.Compiler.Tests")]
[Collection(nameof(SharedCollection))]
public class GeneralTests(SharedValue sharedValue)
{
    private readonly Dictionary<string, ExpressionResult> _expressionResults = sharedValue.Results;

    [Theory]
    [InlineData(1, "int x = 5", "5")]
    [InlineData(2, "double y = 3.14", "3.14")]
    [InlineData(3, "int z = x + 10", "15")]
    [InlineData(4, "double result = y * 2", "6.28")]
    [InlineData(5, "int division = 10 / 2", "5")]
    [InlineData(6, "double complex = (5 + 3) * 2 - 4 / 2", "14")]
    [InlineData(7, "string nome = \"Angelo\"", "Angelo")]
    [InlineData(8, "string sobrenome = \"Belchior\"", "Belchior")]
    [InlineData(9, "string completo = nome + \" \" + sobrenome", "Angelo Belchior")]
    [InlineData(10, "string reduzido = completo - \"Belchior\"", "Angelo ")]
    [InlineData(11, "bool isTrue = true", "true")]
    [InlineData(12, "bool isFalse = false", "false")]
    [InlineData(13, "double raiz = sqrt(16)", "4")]
    [InlineData(14, "double potencia = pow(2, 3)", "8")]
    [InlineData(15, "double minimo = min(10, 20)", "10")]
    [InlineData(16, "double maximo = max(10, 20)", "20")]
    [InlineData(17, "int tamanho = len(\"Angelo\")", "6")]
    [InlineData(18, "double resultado = pow(sqrt(16), 2)", "16")]
    [InlineData(19, "double complexo = max(min(10, 20), sqrt(25))", "10")]
    [InlineData(20, "int tamanhoNome = len(nome + sobrenome)", "14")]
    [InlineData(21, "double erro = sqrt(-1)", "NaN")]
    [InlineData(22, "double erroArgs = pow(2)", "4")]
    [InlineData(23, "int erroLen = len(123)", "3")]
    [InlineData(24, "double parenteses = (5 + 3) * (2 - 1)", "8")]
    [InlineData(25, "double nested = ((10 + 5) * 2) / (3 + 2)", "6")]
    public void Must_Parse_Expressions(
#pragma warning disable xUnit1026
        int order,
#pragma warning restore xUnit1026
        string expression,
        string expectedResult)
    {
        var lexer = new Lexer(expression);
        var tokens = lexer.ExtractTokens();

        var syntaxParser = new SyntaxParser(_expressionResults, tokens);
        var result = syntaxParser.Parse();

        Assert.Equal(expectedResult, result.Value.ToString());
    }

    [Theory]
    [InlineData("sqrt()", "Invalid number of arguments for sqrt")]
    [InlineData("sqrt(1,3)", "Invalid number of arguments for sqrt")]
    [InlineData("sqrt(\"xxx\")", "Can't convert xxx to double")]
    [InlineData("pow()", "Invalid number of arguments for pow")]
    [InlineData("pow(1,2,3)", "Invalid number of arguments for pow")]
    [InlineData("pow(\"xxx\")", "Can't convert xxx to double")]
    [InlineData("min()", "Invalid number of arguments for min")]
    [InlineData("min(1,2,3)", "Invalid number of arguments for min")]
    [InlineData("min(\"xxx\", \"xxx\")", "Can't convert xxx to double")]
    [InlineData("max()", "Invalid number of arguments for max")]
    [InlineData("max(1,2,3)", "Invalid number of arguments for max")]
    [InlineData("max(\"xxx\", \"xxx\")", "Can't convert xxx to double")]
    [InlineData("round()", "Invalid number of arguments for round")]
    [InlineData("round(1,2,3)", "Invalid number of arguments for round")]
    [InlineData("round(\"xxx\", \"xxx\")", "Can't convert xxx to double")]
    [InlineData("random(1,2,3,4)", "Invalid number of arguments for random")]
    [InlineData("random(\"xxx\", \"xxx\")", "Can't convert xxx to int")]
    [InlineData("len()", "Invalid number of arguments for len")]
    [InlineData("len(1,2,3)", "Invalid number of arguments for len")]
    [InlineData("print(1,2,3)", "Invalid number of arguments for print")]
    [InlineData("xpto(abc)", "Variable or Function 'xpto' not declared")]
    public void Invalid_Functions_Must_Throw_Exception(string expression, string exceptionMessage)
    {
        var exception = Assert.Throws<Exception>(() =>
        {
            var results = new Dictionary<string, ExpressionResult>();
            var lexer = new Lexer(expression);
            var tokens = lexer.ExtractTokens();

            var syntaxParser = new SyntaxParser(results, tokens);
            _ = syntaxParser.Parse();
        });

        Assert.Equal(exceptionMessage, exception.Message);
    }
}

public class InlineDataOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
        => testCases.OrderBy(tc => int.TryParse(tc.TestMethodArguments[0].ToString(), out var number) ? number : 0);
}

public class SharedValue
{
    public Dictionary<string, ExpressionResult> Results { get; } = new();
};

[CollectionDefinition(nameof(SharedCollection))]
public class SharedCollection : ICollectionFixture<SharedValue>;