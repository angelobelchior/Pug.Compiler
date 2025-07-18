const language = 'pug-lang';

let editor;
let initialCode;

let appConsole = document.getElementById('app-console');
let tokens = document.getElementById('tokens');
let identifiers = document.getElementById('identifiers');
let variables = document.getElementById('variables');
let currentDecorations = [];
let errorDecorations = [];

document.addEventListener('DOMContentLoaded', async function () {
    configureEditor();
    configureRunButton();
    await loadSamples();
})

function configureEditor() {
    require.config({paths: {vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.39.0/min/vs'}});
    require(['vs/editor/editor.main'], function () {
        monaco.languages.register({id: language});
        monaco.languages.setMonarchTokensProvider(language, {
            tokenizer: {
                root: [
                    [/\/\/.*/, 'comment'],
                    [/\b(log|exp|sin|cos|tan|atn|abs|sgn|left|right|mid|trim|trim_end|trim_start|to_string|to_bool|to_int|to_double|print|max|min|sqrt|random|pow|round|upper|lower|len|replace|substr|clear|iif)\b/, 'function'],
                    [/\b(true|false|func|if|else|end)\b/, 'keyword'],
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
                {token: 'function', foreground: '#39CC9A'},
                {token: 'keyword', foreground: '#569CD6'},
                {token: 'type', foreground: '#569CD6'},
                //{token: 'comment', foreground: '#8FBC8F'}
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
            value: initialCode,
            language: language,
            theme: 'pug-dark',
            fontSize: 16
        });
    });
}

function configureRunButton() {
    document.getElementById('run-button').addEventListener('click', async function () {
        const codeValue = editor.getValue();
        try {
            editor.setPosition({lineNumber: 1, column: 1});
            currentDecorations = editor.deltaDecorations(currentDecorations, []);
            errorDecorations = editor.deltaDecorations(errorDecorations, []);

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
            setVariables(result);
            setIdentifiers(result);

            if (result.hasError)
                setError(result);

        } catch (error) {
            alert(error);
        }
    });
}

function setCode(result) {
    appConsole.innerText = result.output.map(line => `> ${line}`).join('\n');
}

function setTokens(result) {
    tokens.innerHTML = '';
    for (const token of result.tokens) {
        const li = createLi(token.value, token);
        tokens.append(li);
    }
    document.getElementById("tokensText").innerHTML = `Tokens (${result.tokens.length})`;
}

function setVariables(result) {
    variables.innerHTML = '';
    for (const variable of result.variables) {
        const li = createLi(`${variable.dataType} ${variable.name} ${variable.value}`, null);
        variables.append(li);
    }
    document.getElementById("variablesText").innerHTML = `Variables (${result.variables.length})`;
}

function setIdentifiers(result) {
    identifiers.innerHTML = '';
    for (const identifier of result.identifiers) {
        const li = createLi(`${identifier.value} as ${identifier.dataType}`, null);
        identifiers.append(li);
    }
    document.getElementById("identifiersText").innerHTML = `Identifiers (${result.identifiers.length})`;
}

function setDecoration(range, className, parameters, isWholeLine) {
    return editor.deltaDecorations(parameters, [
        {
            range: range,
            options: {
                isWholeLine: isWholeLine,
                className: className
            }
        }
    ]);
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
    const range = getRangeByToken(token);
    currentDecorations = setDecoration(range, 'token-highlight', currentDecorations, false);
    editor.setSelection(range);
}

function setError(result) {
    if (!result.currentToken) return;
    const range = getRangeByToken(result.currentToken);
    errorDecorations = setDecoration(range, 'error-line', errorDecorations, true);
    currentDecorations = setDecoration(range, 'token-highlight', currentDecorations, false);
}

function getRangeByToken(token) {
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
    if (token.type === 'String') // por causa das aspas
        quotationCount = 2;

    const endIndex = startIndex + token.value.length + quotationCount;
    return new monaco.Range(line, startIndex, line, endIndex);
}

async function loadSamples() {
    try {
        const response = await fetch('/samples', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const samples = await response.json();

        const selectElement = document.getElementById('code-samples');
        selectElement.innerHTML = '';

        samples.forEach(sample => {
            const option = document.createElement('option');
            option.value = sample.code;
            option.textContent = sample.title;
            selectElement.appendChild(option);
        });

        selectElement.addEventListener('change', function () {
            const selectedCode = selectElement.value;
            editor.setValue(selectedCode);
        });

        initialCode = samples[0].code;
    } catch (error) {
        console.error('Erro ao carregar samples:', error);
    }
}