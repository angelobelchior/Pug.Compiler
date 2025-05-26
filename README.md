# Pug.Compiler

Esse repositório faz parte da série [Reinventando a Roda: Criando um compilador em csharp](https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-1-2gih) na qual vamos construir um compilador do zero em **csharp**.

Essa construção será dividida em partes e cada parte terá um post e uma branch específica. 

Cada post vai conter a explicação e implementação de uma ou mais funcionalidades do compilador e sempre estará associado a uma branch (parte1, parte2, etc.)


A [branch main](https://github.com/angelobelchior/Pug.Compiler) sempre estará com o código referente ao post mais recente. Nesse caso estamos na [parte 2](https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-2-2jmi).

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
0.43447518061452317
> random(1, 10)
4
> random(5)
3
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
