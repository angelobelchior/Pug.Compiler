# Pug.Compiler

Esse repositório faz parte da série [Reinventando a Roda: Criando um compilador em csharp](https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-1-2gih) na qual vamos construir um compilador do zero em **csharp**.

Essa construção será dividida em partes e cada parte terá um post e uma branch específica. 

Cada post vai conter a explicação e implementação de uma ou mais funcionalidades do compilador e sempre estará associado a uma branch (parte1, parte2, etc.)


A [branch main](https://github.com/angelobelchior/Pug.Compiler) sempre estará com o código referente ao post mais recente. Nesse caso estamos na [parte 1](https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-1-2gih);

----

## Parte 1

**Link para o Post:** 
- https://dev.to/angelobelchior/reinventando-a-roda-criando-um-compilador-em-csharp-parte-1-2gih

**Link para a Branch:** 
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
