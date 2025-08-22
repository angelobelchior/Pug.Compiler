namespace Pug.Compiler.Editor.Endpoints.Models;

[ExcludeFromCodeCoverage]
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

            new("While Loop",
                """
                int i = 0
                while i < 5
                    print("i is " + i)
                    i = i + 1
                end
                print("Loop finished")
                """
            ),

            new("Even or Odd",
                """
                int i
                while i < 5
                    if i % 2 == 0
                        print(i + " é par")
                    else
                        print(i + " é ímpar")
                    end
                    i = i + 1
                end
                """
            ),
            
            new("Is it a prime number?",
                """
                int n = 2
                while n <= 10
                    int divisor = 2
                    bool isPrime = true
                    while divisor < n
                        if n % divisor == 0
                            isPrime = false
                        end
                        divisor = divisor + 1
                    end
                    if isPrime == true
                        print(n + " is prime")
                    else
                        print(n + " is not prime")
                    end
                    n = n + 1
                end
                """
            ),
            
            new("Palindrome Number",
                """
                int original = 213312
                int numero = original
                int invertido = 0
                while numero > 0
                    int digito = numero % 10
                    invertido = invertido * 10 + digito
                    numero = to_int(numero / 10)
                end
                if original == invertido
                    print(original + " is a palindrome")
                else
                    print(original + " is not a palindrome")
                end
                """
            ),
            
            new("Convert Decimal to Binary",
                """
                int numero = 1984
                string binario = ""
                while numero > 0
                    int resto = numero % 2
                    binario = resto + binario
                    numero = to_int(numero / 2)
                end
                print("Binário = " + binario)
                """
            ),
            
            new("Reverse String",
                """
                string texto = "angelo"
                int i = len(texto) - 1
                string invertido = ""
                while i >= 0
                    invertido = invertido + char_at(texto, i)
                    i = i - 1
                end
                print("Texto invertido = " + invertido)
                """
            ),
            
            new("Ceulsius to Fahrenheit and vice versa",
                """
                double celsius = 25
                double fahrenheit = (celsius * 9 / 5) + 32
                print("Celsius para Fahrenheit: " + fahrenheit)
                
                double f = 77
                double c = (f - 32) * 5 / 9
                print("Fahrenheit para Celsius: " + c)
                """
            ),

            new("Table Multiplication",
                """
                int i = 1
                while i <= 10
                    int j = 1
                    while  j <= 10
                        print(i + "x" + j + "=" + i * j)
                        j = j + 1
                    end
                    print("=========")
                    i = i + 1
                end
                """
            ),

            new("Factorial",
                """
                int n = 10          
                int result = 1     
                int i = 1          

                while i <= n
                    result = result * i
                    i = i + 1
                end
                print("Fatorial de " + n + " = " + result)
                """
            ),

            new("Fibonacci",
                """
                int a = 0
                int b = 1
                int count = 1

                while count <= 10
                    print("Fibonacci(" + count + ") = " + a)
                    int temp = a + b
                    a = b
                    b = temp
                    count = count + 1
                end
                """),
            
            new("For: Even or Odd",
                """
                for int i = 1 to 5
                    if i % 2 == 0
                        print(i + " é par")
                    else
                        print(i + " é ímpar")
                    end
                end
                """
            ),

            new("Reverse String",
                """
                string texto = "angelo"
                int i = len(texto) - 1
                string invertido = ""
                while i >= 0
                    invertido = invertido + char_at(texto, i)
                    i = i - 1
                end
                print("Texto invertido = " + invertido)
                """
            ),

            new("For: Table Multiplication",
                """
                for int i = 1 to 10
                    for int j = 1 to 10
                        print(i + "x" + j + "=" + i * j)
                    end
                    print("-------")
                end
                """
            ),

            new("For: Factorial",
                """
                int n = 10          
                int result = 1     

                for int i = 1 to n
                    result = result * i
                end
                print("Fatorial de " + n + " = " + result)
                """
            ),

            new("For: Fibonacci",
                """
                int a = 0
                int b = 1

                for int i = 1 to 10
                    print("Fibonacci(" + i + ") = " + a)
                    int temp = a + b
                    a = b
                    b = temp
                end
                """),

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