namespace Pug.Compiler.Editor.Endpoints.Models;

public record Sample(string Title, string Code)
{
    public static IReadOnlyList<Sample> CreateSamples()
    {
        var samples = new List<Sample>
        {
            new("Hello World", 
                """
                print("Hello, World!")
                """),
            
            new("Comments", 
                """
                // Print a message to the console
                int x = 10 // Variable declaration
                print(x) // Output the value of x
                """),

            new(
                "Variables",
                """
                int x = 10
                print(x)
                double pi = 3.14159
                print(pi)
                string name = "Pug"
                print(name)
                bool isTrue = true
                print(isTrue)
                print("x is " + x + ", pi is " + pi + ", name is " + name + ", isTrue is " + isTrue)
                """
            ),
            
            new("Conditionals",
                """
                int x = 10
                if x > 5
                    print("x is greater than 5")
                else    
                    print("x is 5 or less")
                end
                """
            ),
            
            new("Nested Conditionals",
                """
                int idade = 18
                string nome = "Angelo"
                if idade >= 18
                   print("Acesso permitido")
                else
                   if nome == "Angelo"
                       print("Acesso em avaliação")
                   else
                       print("Acesso negado")
                   end
                end
                print("Fim do programa")
                """
            ),
            
            new("Built-In Functions",
                """
                int raiz = sqrt(4)
                print("A raiz quadrada de 4 é " + raiz)
                print("---")
                int potencia = pow(2, 3)
                print("2 elevado a 3 é " + potencia)
                print("---")
                int menor = min(10, 5)
                print("O menor valor entre 10 e 5 é " + menor)
                print("---")
                int maior = max(10, 5)
                print("O maior valor entre 10 e 5 é " + maior)
                print("---")
                double arredondado = round(3.14159, 2)
                print("3.14159 arredondado para 2 casas decimais é " + arredondado)
                print("---")
                int aleatorio = random(1, 100)
                print("Número aleatório entre 1 e 100: " + aleatorio)
                print("---")
                string maiusculo = upper("pug")
                print("Maiúsculo: " + maiusculo)
                print("---")
                string minusculo = lower("PUG")
                print("Minúsculo: " + minusculo)
                print("---")
                int tamanho = len("Pug")
                print("Tamanho da string 'Pug': " + tamanho)
                print("---")
                string substituido = replace("Pug é incrível", "incrível", "fantástico")
                print("Substituição: " + substituido)
                print("---")
                string substring = substr("Pug é incrível", 4, 2)
                print("Substring: " + substring)
                print("---")
                string condicional = iif(10 > 5, "Verdadeiro", "Falso")
                print("Resultado do iif: " + condicional)
                print("---")
                """
            ),
            
            new("Token Error", 
                """
                var x = 10
                print(x)
                """),
            
            new("Syntax Error", 
                """
                int x = 10
                if x > // 5
                    print("where is 5?")
                else    
                    print("x is 5 or less")
                // end
                print("ihuuuu....")
                """),
        };

        return samples;
    }
}