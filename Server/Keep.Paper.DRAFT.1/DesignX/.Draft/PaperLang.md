PaperLang
=========

Sintaxe:

    STATEMENT

Sample:

    open (Users)
      on (new) as (Novo Usuário)
           popup (User.New)
              on (commit) as (Salvar)
                   with (current)
                   perform (User.Create)
                     when (view)
                       # Fechamos o popup e atualizamos a visão.
                       unstack view (User.New)
                       refresh (Users)
      on (delete) per row as (Remover Usuários)
           with (current)
           perform (User.Delete)
             when (view)
               # E a mensagem de confirmacao?
               do nothing
      ;
STATEMENT
:
    PERFORM | OPEN | STACK | POPUP

PERFORM
:   Realiza uma requisição e processa a resposta.

        [ with (VIEW, ...) ] [ with cache ] perform (RESOURCE) [ [ when (RESULT) PROCEED ] ... ] [ else PROCEED ] ;
    
OPEN
:   Realiza uma requisição e substitui a visão corrente pela visão recebida.

        [ with (VIEW, ...) ] open (RESOURCE) [ BIND ] ;
        
    O mesmo que:
    
        [ with (VIEW, ...) ] with cache perform (RESOURCE) when (view) set view [ BIND ];

STACK
:   Realiza uma requisição e exibe a visão recebida por cima da visão corrente.

        [ with (VIEW, ...) ] stack (RESOURCE) [ BIND ] ;
        
    O mesmo que:
    
        [ with (VIEW, ...) ] with cache perform (RESOURCE) when (view) stack view [ BIND ];

POPUP
:   Realiza uma requisição e exibe a visão recebida por cima da visão corrente como um popup.

        [ with (VIEW, ...) ] popup (RESOURCE) [ BIND ] ;
        
    O mesmo que:
    
        [ with (VIEW, ...) ] with cache perform (RESOURCE) when (view) set view on top [ BIND ];

BIND
:   Conecta um evento da visão a um comando.

        on (EVENT) [ per row] [ as (TITLE) ] STATEMENT ;

PROCEED
:   Um comando de processamento do resultado.

        STATEMENT
        continue
        report [ (MESSAGE) ] [ severity (SEVERITY) ]
        refresh [ (VIEW) ]
        set view [ on top ] [ BIND ]
        replace view [ on top ] [ BIND ]
        stack view [ on top ] [ BIND ]
        unstack view [ [ until ] (VIEW) ]
        unstack all

EVENT
:   Um nome de evento disparado pela visão.
    
RESOURCE
:   A resource PATH, like:

    -   MyResource
    -   MyResource(Key1=Value1;...)

VIEW
:   A resource PATH or PATTERN stacked.
    It can be:

    -   PATH
    -   PATH[n]
    -   PATH[-n]
    -   *
    -   *[n]
    -   *[-n]
    -   current
        -   O mesmo que *

    When:
    -   `n` matches the top of the stack.
    -   `-n` matches the base of the stack.

PATH
:   O nome de um recurso mais suas chaves, na forma:

        Nome(Key1=Value1,...)

    Exemplos:

    -   MyResource
    -   MyResource(Id=10)

MESSAGE
:   Any text.

TEXT
:   Any text.

SEVERITY
:   Severidade da ocorrência. Um de:

    -   danger
    -   warning
    -   success
    -   default
    -   information
    -   trace
