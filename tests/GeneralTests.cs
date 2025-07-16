using Pug.Compiler.CodeAnalysis;
using Pug.Compiler.Runtime;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Pug.Compiler.Tests;

[TestCaseOrderer("Pug.Compiler.Tests." + nameof(InlineDataOrderer), "Pug.Compiler.Tests")]
[Collection(nameof(SharedCollection))]
public class GeneralTests(SharedValue sharedValue)
{
    private readonly Dictionary<string, Identifier> _expressionResults = sharedValue.Results;

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
    [InlineData(11, "bool isTrue = true", "True")]
    [InlineData(12, "bool isFalse = false", "False")]
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
    [InlineData(26, "double a", "0")]
    [InlineData(27, "int b", "0")]
    [InlineData(28, "string c", "")]
    [InlineData(29, "bool d", "False")]
    [InlineData(30, "int idade", "0")]
    [InlineData(31, "idade = idade + 1", "1")]
    [InlineData(32, "string nome = \"Angelo\"", "Angelo")]
    [InlineData(33, "nome = nome + \" Belchior\"", "Angelo Belchior")]
    [InlineData(34, "\"A\" * 3", "AAA")]
    [InlineData(35, "3 * \"A\"", "AAA")]
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
        var result = syntaxParser.Evaluate();

        if (expectedResult == "\0")
            Assert.Empty(result);
        else
            Assert.Equal(expectedResult, result[0].Value.ToString());
    }

    [Theory]
    [InlineData("\"A\" * 3", "AAA")]
    [InlineData("3 * \"A\"", "AAA")]
    [InlineData("\"A\" + 3", "A3")]
    [InlineData("3 + \"A\" //esse comentário", "3A")]
    [InlineData("//este é um comentário", "\0")]
    [InlineData("string texto = \"Esse texto tem // barras\"", "Esse texto tem // barras")]
    [InlineData("1==1", "True")]
    [InlineData("1!=1", "False")]
    [InlineData("\"a\"==\"a\"", "True")]
    [InlineData("\"a\"!=\"a\"", "False")]
    [InlineData("\"a\"==\"b\"", "False")]
    [InlineData("\"a\"!=\"b\"", "True")]
    [InlineData("true==true", "True")]
    [InlineData("true!=true", "False")]
    [InlineData("false==false", "True")]
    [InlineData("false!=false", "False")]
    [InlineData("1 == (2*3)", "False")]
    [InlineData("4 == pow(2)", "True")]
    [InlineData("3 == pow(2) - 1", "True")]
    [InlineData("4 == pow(2) - 1", "False")]
    [InlineData("true && true", "True")]
    [InlineData("true && false", "False")]
    [InlineData("false && false", "False")]
    [InlineData("false && true", "False")]
    [InlineData("true || true", "True")]
    [InlineData("true || false", "True")]
    [InlineData("false || false", "False")]
    [InlineData("false || true", "True")]
    public void Must_Parse_Operations(
        string expression,
        string expectedResult)
    {
        var lexer = new Lexer(expression);
        var tokens = lexer.ExtractTokens();

        var syntaxParser = new SyntaxParser(_expressionResults, tokens);
        var result = syntaxParser.Evaluate();

        if (expectedResult == "\0")
            Assert.Empty(result);
        else
            Assert.Equal(expectedResult, result[0].Value.ToString());
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
    [InlineData("xpto(abc)", "Unknown identifier: xpto")]
    [InlineData("int x = false", "Invalid type bool. Expected a int")]
    [InlineData("int x = \"abcd\"", "Invalid type string. Expected a int")]
    [InlineData("double x = false", "Invalid type bool. Expected a double")]
    [InlineData("double x = \"abcd\"", "Invalid type string. Expected a double")]
    [InlineData("bool x = 12234", "Invalid type int or double. Expected a bool")]
    [InlineData("bool x = \"abcd\"", "Invalid type string. Expected a bool")]
    [InlineData("1 == \"abcd\"", "Cannot apply Equal operator to different types: Double and String")]
    [InlineData("1 != \"abcd\"", "Cannot apply NotEqual operator to different types: Double and String")]
    public void Invalid_Functions_Must_Throw_Exception(
        string expression,
        string exceptionMessage)
    {
        var exception = Assert.Throws<SyntaxParserException>(() =>
        {
            var results = new Dictionary<string, Identifier>();
            var lexer = new Lexer(expression);
            var tokens = lexer.ExtractTokens();

            var syntaxParser = new SyntaxParser(results, tokens);
            _ = syntaxParser.Evaluate();
        });

        Assert.Equal(exceptionMessage, exception.Message);
    }

    [Theory]
    [InlineData("true && true", "True")]
    [InlineData("true && false", "False")]
    [InlineData("false && false", "False")]
    [InlineData("false && true", "False")]
    [InlineData("true || true", "True")]
    [InlineData("true || false", "True")]
    [InlineData("false || false", "False")]
    [InlineData("false || true", "True")]
    [InlineData("pow(2)  == 4 && min (3, 2) == 2", "True")]
    [InlineData("sqrt(25) == 5 && max(1, 7) == 7", "True")]
    [InlineData("len(\"abc\") == 3 && round(2.7) == 3", "True")]
    [InlineData("pow(2) == 5 && sqrt(16) == 4", "False")]
    [InlineData("min(8,3) == 8 || max(1,7) == 10", "False")]
    [InlineData("pow(3) == 9 || len(\"dog\") == 4", "True")]
    [InlineData("sqrt(36) == 7 || round(1.4) == 1", "True")]
    [InlineData("upper(\"dog\") == \"DOG\" && lower(\"CAT\") == \"cat\"", "True")]
    [InlineData("replace(\"abc\", \"b\", \"d\") == \"abc\" || pow(2,3) == 8", "True")]
    [InlineData("round(2.51) == 3 && len(\"pug\") == 4", "False")]
    [InlineData("1 == 1", "True")]
    [InlineData("1 != 2", "True")]
    [InlineData("3 > 2", "True")]
    [InlineData("4 >= 4", "True")]
    [InlineData("5 < 10", "True")]
    [InlineData("7 <= 7", "True")]
    [InlineData("(2 < 3) && (4 > 1)", "True")]
    [InlineData("(3 == 3) || (6 != 6)", "True")]
    [InlineData("sqrt(25) == 5", "True")]
    [InlineData("pow(3) == 9", "True")]
    [InlineData("min(8, 3) == 8", "False")]
    [InlineData("max(1, 7) > 10", "False")]
    [InlineData("round(2.49) == 2", "True")]
    [InlineData("round(2.51) == 2", "False")]
    [InlineData("len(\"pug\") == 3", "True")]
    [InlineData("replace(\"abc\", \"b\", \"d\") == \"adc\"",  "True")]
    [InlineData("upper(\"dog\") == \"DOG\"", "True")]
    [InlineData("lower(\"CAT\") != \"cat\"", "False")]
    public void Must_Parse_Logical_Expressions(
        string expression,
        string expectedResult)
    {
        var lexer = new Lexer(expression);
        var tokens = lexer.ExtractTokens();

        var syntaxParser = new SyntaxParser(_expressionResults, tokens);
        var result = syntaxParser.Evaluate();

        Assert.Single(result);
        Assert.Equal(expectedResult, result[0].Value.ToString());
    }

    [Fact]
    public void Must_Throw_Exception_When_FunctionOrVariable_Not_Declared()
    {
        var exception = Assert.Throws<SyntaxParserException>(() =>
        {
            var results = new Dictionary<string, Identifier>();
            var lexer = new Lexer("int idade = 10");
            var tokens = lexer.ExtractTokens();

            var syntaxParser = new SyntaxParser(results, tokens);
            var result = syntaxParser.Evaluate();

            Assert.Equal(10, result[0].Value);

            lexer = new Lexer("idade = y");
            tokens = lexer.ExtractTokens();

            syntaxParser = new SyntaxParser(results, tokens);
            _ = syntaxParser.Evaluate();
        });

        Assert.Equal("Unknown identifier: y", exception.Message);
    }

    [InlineData(
        """
        int idade = 18
        string nome = "Angelo"
        if idade >= 18
            string a = "acesso permitido"
        else
            if nome == "Angelo"
                string b = "acesso em avaliação"
            else
                string c = "acesso negado"
            end
        end
        string d = "Fim do programa"
        """,
        "18", "Angelo", "acesso permitido", "Fim do programa"
    )]
    
    [InlineData(
        """
        int idade = 41
        if idade == 41
            string a = "igual"
        else
            string a = "diferente"
        end
        """,
        "41", "igual"
    )]
    
    [InlineData(
        """
        int idade = 22
        if idade == 41
           string a = "igual"
        else
            string b = "diferente"
        end
        """,
        "22", "diferente"
    )]
    
    [InlineData(
        """
        int idade = 30
        bool cadastrado = true
        if idade >= 18 && cadastrado == true
            string a = "acesso liberado"
        else
            string b = "acesso negado"
        end
        """,
        "30", "True", "acesso liberado"
    )]
    
    [InlineData(
        """
        int a = 5
        int b = 10
        if (a + b) == 15
            string c = "soma correta"
        else
            string d = "soma incorreta"
        end
        """,
        "5", "10", "soma correta"
    )]
    
    [InlineData(
        """
        int idade = 15
        if idade >= 0
            if idade < 12
                string a = "criança"
            else
                if idade < 18
                    string b = "adolescente"
                else
                    string c = "adulto"
                end
            end
        else
            string d = "idade inválida"
        end
        """,
        "15", "adolescente"
    )]
    
    [InlineData(
        """
        int idade = 10
        if idade >= 0
            if idade < 12
                string a = "criança"
            else
                if idade < 18
                    string b = "adolescente"
                else
                    string c = "adulto"
                end
            end
        else
            string d = "idade inválida"
        end
        """,
        "10", "criança"
    )]
    
    [InlineData(
        """
        int idade = 19
        if idade >= 0
            if idade < 12
                string a = "criança"
            else
                if idade < 18
                    string b = "adolescente"
                else
                    string c = "adulto"
                end
            end
        else
            string d = "idade inválida"
        end
        """,
        "19", "adulto"
    )]
    
    [InlineData(
        """
        int idade = -10
        if idade >= 0
            if idade < 12
                string a = "criança"
            else
                if idade < 18
                    string b = "adolescente"
                else
                    string c = "adulto"
                end
            end
        else
            string d = "idade inválida"
        end
        """,
        "-10", "idade inválida"
    )]
    
    [Theory]
    public void Must_Execute_If_Statement(
        string code,
        params string[] variables)
    {
        var identifiers = new Dictionary<string, Identifier>();
        var lexer = new Lexer(code);
        var tokens = lexer.ExtractTokens();
    
        var syntaxParser = new SyntaxParser(identifiers, tokens);
        _ = syntaxParser.Evaluate();
    
        Assert.Equal(variables.Length, identifiers.Count);
        for (var i = 0; i < variables.Length; i++)
        {
            var a = variables[i];
            var b = identifiers.Values.ElementAt(i).Value.ToString();
            Assert.Equal(a, b);
        }
    }
}

public class InlineDataOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
        => testCases.OrderBy(tc =>
        {
            if (tc.TestMethodArguments is not null && tc.TestMethodArguments.Length > 0)
                return int.TryParse(tc.TestMethodArguments[0].ToString(), out var number) ? number : 0;

            return 0;
        });
}

public class SharedValue
{
    public Dictionary<string, Identifier> Results { get; } = new();
}

[CollectionDefinition(nameof(SharedCollection))]
public class SharedCollection : ICollectionFixture<SharedValue>;