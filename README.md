# Pug.Compiler

[![.NET](https://github.com/angelobelchior/Pug.Compiler/actions/workflows/dotnet.yml/badge.svg)](https://github.com/angelobelchior/Pug.Compiler/actions/workflows/dotnet.yml)

Esse repositório faz parte da série [Reinventando a Roda: Criando um compilador em csharp](https://dev.to/angelobelchior/series/31549) na qual vamos construir um compilador do zero em **csharp**.

Essa construção será dividida em partes e cada parte terá um post e uma branch específica.

Cada post vai conter a explicação e implementação de uma ou mais funcionalidades do compilador e sempre estará associado a uma branch (parte1, parte2, etc.).


A [branch main](https://github.com/angelobelchior/Pug.Compiler) sempre estará com o código referente ao post mais recente. Nesse caso estamos na [parte 4](https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-4-304d).

----

## Parte 4

**Link para o Post:**
- https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-4-304d

**Link para a Branch:**
- https://github.com/angelobelchior/Pug.Compiler/tree/parte4

**Funcionalidades:**
- Suporte a operadores lógicos e condicionais
- **Pug.Editor** um editor de código para a linguagem Pug

Nessa branch adicionamos o suporte a operadores lógicos e condicionais, além de um editor de código configurado para suportar a linguagem _Pug_.

```bash
> bool valor = 2 == 2 || 3 == 4
True
> bool valor = 1 == 2 && 3 == 4
False
> bool valor = (5 > 1) && (10 > 2)
True
> bool valor = (5 < 1) || (10 < 2)
False
> (2 < 3) && (4 > 1)
True
> (3 == 3) || (6 != 6)
True
> 7 <= 7
True
> 3 <= 2
False
```

![Pug.Editor](https://media2.dev.to/dynamic/image/width=800%2Cheight=%2Cfit=scale-down%2Cgravity=auto%2Cformat=auto/https%3A%2F%2Fdev-to-uploads.s3.amazonaws.com%2Fuploads%2Farticles%2Fm58yef7jceq45qcenxen.png)

----

## Parte 3

**Link para o Post:**
- https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-3-17n0

**Link para a Branch:**
- https://github.com/angelobelchior/Pug.Compiler/tree/parte3

**Funcionalidades:**
- Adicionando suporte a declaração de variáveis

Nessa branch adicionamos o suporte a declaração de variáveis e atribuição de valores a elas:

```bash
> int idade = 41
41
> string nome = "Angelo"
Angelo
> print(nome + " tem " + idade + " de idade")
Angelo tem 41 de idade
> double pi = 3.14
3.14
> "A" * 3
AAA
> 3 * "A"
AAA
> "Angelo Belchior" - " Belchior"
Angelo
```
----

## Parte 2

**Link para o Post:**
- https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-2-2jmi

**Link para a Branch:**
- https://github.com/angelobelchior/Pug.Compiler/tree/parte2

**Funcionalidades:**
- Adicionando métodos internos à nossa linguagem

Nessa branch adicionamos o suporte a funções internas da nossa linguagem como `pow`, `sqrt`, `min`, `max`, `round` e `random`.
E como podemos ver nos exemplos abaixo, é possível passar expressões matemáticas como parâmetros:

```bash
> pow(2)
4
> pow(2, 3)
8
> sqrt(20 + 5)
5
> min(7, sqrt(25) + 1)
6
> max(10, 11)
11
> round(12.35)
12
> round(12.35, 1)
12.4
> random()
18
> random(1, 10)
4
> random(5)
3
> int valor = random(pow(2), sqrt(81))
6
```  
----

## Parte 1

**Link para o Post:**
- https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-1-2gih

**Link para o PR:**
- https://github.com/angelobelchior/Pug.Compiler/tree/parte1

**Funcionalidades:**
- Interpretação de Expressões Numéricas

Nessa branch temos o motor de interpretação de expressões numéricas que respeitam a ordem dos parênteses e a prioridade das operações de multiplicação e divisão:

Podemos executar as seguintes expressões:

```bash
> 1 + 2
3

> 1 + (2 * 3)
7

> (2 * (3 * 4) / 5) + 6
10,8
```
