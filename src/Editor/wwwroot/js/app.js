const language = 'pug-lang';

let editor;
let code =
    `int idade = 18
string nome = "Angelo"
if idade >= 18 then
    print("acesso permitido")
else
    if nome == "Angelo" then
        print("acesso em avaliação")
    else
        print("acesso negado")
    end
end
print("Fim do programa")
`;


function configureEditor() {
    require.config({paths: {vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.39.0/min/vs'}});
    require(['vs/editor/editor.main'], function () {
        monaco.languages.register({id: language});
        monaco.languages.setMonarchTokensProvider(language, {
            tokenizer: {
                root: [
                    [/\/\/.*/, 'comment'],
                    [/\b(print|max|min|sqrt|random|pow|round|upper|lower|len|replace|substr|clear)\b/, 'function'],
                    [/\b(true|false|func|if|else|end|then)\b/, 'keyword'],
                    [/\b(int|string|double|bool)\b/, 'type'],
                    [/\b(==|=|\+|-|\*|\/|%|&&|\|\|)\b/, 'operator'],
                    [/\b\d+\b/, 'number'],
                    [/"([^"\\]|\\.)*"/, 'string'],
                    [/[a-zA-Z_][a-zA-Z0-9_]*/, 'identifier']
                ]
            }
        });
        monaco.editor.defineTheme('pug-dark', {
            base: 'vs-dark',
            inherit: true,
            rules: [
                {token: 'function', foreground: '#6fA076'},
                {token: 'keyword', foreground: '#569CD6'},
                {token: 'type', foreground: '#569CD6'},
            ],
            colors: {
                'editor.foreground': '#FFFFFF',
                'editor.background': '#1E1E1E'
            }
        });
        monaco.languages.setLanguageConfiguration(language, {
            comments: {
                lineComment: '//',
            },
            brackets: [
                ['[', ']']
            ],
            autoClosingPairs: [
                {open: '"', close: '"', notIn: ['string']},
                {open: '(', close: ')', notIn: ['string']}
            ],
            folding: {
                markers: {
                    start: /^(if|else|func|end|then|return)\b/,
                    end: /^(end)\b/
                }
            }
        });
        editor = monaco.editor.create(document.getElementById('container'), {
            value: code,
            language: language,
            theme: 'pug-dark',
            fontSize: 16
        });
    });
}

let appConsole = document.getElementById('app-console');
let tokens = document.getElementById('tokens');
let identifiers = document.getElementById('identifiers');
let currentDecorations = [];

document.addEventListener('DOMContentLoaded', function () {

    configureEditor();

    document.getElementById('run-button').addEventListener('click', async function () {
        const codeValue = editor.getValue();
        try {
            const response = await fetch('/compile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({code: codeValue})
            });
            const result = await response.json();
            setCode(result);
            setTokens(result);
            setIdentifiers(result);

        } catch (error) {
            console.error('Erro ao compilar:', error);
        }
    });
})

function setCode(result) {
    appConsole.innerText = result.output.map(line => `> ${line}`).join('\n');
    console.log(result);
}

function setTokens(result) {
    tokens.innerHTML = '';
    for (const token of result.tokens) {
        const li = createLi(token.value, token);
        tokens.append(li);
    }
}

function setIdentifiers(result) {
    identifiers.innerHTML = '';
    for (const identifier of result.identifiers) {
        const li = createLi(`${identifier.value} as ${identifier.dataType}`, null);
        identifiers.append(li);
    }
}

function createLi(content, token) {
    let li = document.createElement('li');
    li.className = 'list-group-item';
    li.innerText = content;
    li.style.backgroundColor = '#cccccc';
    if (token) {
        li.style.cursor = 'pointer';
        li.style.textDecoration = 'underline';
        li.style.color = 'blue';
        li.onclick = () => {
            selectCode(token);
        };
    }
    return li;
}

function selectCode(token) {
    const lines = editor.getValue().split('\n');
    let startIndex = token.position + 1;

    let line = 1;
    let offset = 0;

    for (let i = 0; i < lines.length; i++) {
        if (startIndex <= offset + lines[i].length) {
            line = i + 1;
            startIndex = startIndex - offset;
            break;
        }
        offset += lines[i].length + 1;
    }

    let quotationCount = 0;
    if(token.type === 'String')
        quotationCount = 2;
    
    const endIndex = startIndex + token.value.length + quotationCount;
    const range = new monaco.Range(line, startIndex, line, endIndex);

    currentDecorations = editor.deltaDecorations(currentDecorations, [
        {
            range: range,
            options: {
                inlineClassName: 'token-highlight'
            }
        }
    ]);
    
    editor.setSelection(range);
}