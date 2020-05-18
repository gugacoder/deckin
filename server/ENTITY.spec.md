# Entity

Entity é qualquer objeto trafegado pela rede.
Preferencialmente em formato JSON.

## Estrutura Geral

CATALOG ::=
    Nome da organização lógica das entidades.

    Geralmente corresponde ao nome de um assembly contento os tipos de
    entidades exportadas.

TYPE ::=
    Tipo da entidade.

    Geralmente corresponde a um nome de tipo do C# mais seu namespace.

PATH ::=
    Caminho de acesso ao conteúdo da entidade.
    
    /Api/VERSION/Entities/CATALOG/TYPE/ID

    O curinga ! existe para abreviação de Api/LATEST/Entities, como segue:
    /!/CATALOG/TYPE/ID

ENTITY-REF ::=
    Representa uma referência de entidade.

    {
      "type": "attachment" | "record" | "table" | "list" | "card" | "cards" | "dashboard" | ...
      "href": URI | PATH
    }

ENTITY ::=
    Representa a entidade e seus metadados.

    {
      "type": "attachment" | "record" | "table" | "list" | "card" | "cards" | "dashboard" | ...
      "data": { ... }
      "meta": { ... }
      "rel": [ ... ]
      "links": {
        "self": {
          "href": URI | PATH
          ...
        }
        ...
      }
      "views": [ VIEW, ... ]
      "actions": [ ACTION, ... ]
      "fields": [ FIELD, ... ]
      "entities": [ ENTITY, ... ]
    }

    Notas:
    -   O campo "data" contém as propriedades do objeto.
    -   O campo "meta" contém os metadados do objeto.
    -   O campo "fields" declara os campos conhecidos da entidade ou da ação.
    -   Para tipos de entidade que representam coleções os itens dessas coleções são
        aquelas entidades que não possuem relacionamento definido ou possuem uma
        lista de relacionamento vazia.

NAME :==
    Um nome qualquer de identificação de uma entidade.

VIEW ::=
    Contém os parâmetros de renderização da entidade.

    VIEW é um ENTITY e por isso tem exatamente a mesma estrutura.
    Algumas restrições se aplicam:
    1.  Omite-se o campo "type" que vale "view" por padrão.
    2.  A propriedade "name" é obrigatória.

ACTION ::=
    Representa uma ação executada em cima de uma entidade.

    ACTION é um ENTITY e por isso tem exatamente a mesma estrutura.
    Algumas restrições se aplicam:
    1.  Omite-se o campo "type" que vale "action" por padrão.
    2.  A propriedade "name" é obrigatória.

FIELD ::=
    Descreve um campo de uma entidade ou de uma ação.

    FIELD é um ENTITY e por isso tem exatamente a mesma estrutura.
    Algumas restrições se aplicam:
    1.  Omite-se o campo "type" que vale "field" por padrão.
    2.  A propriedade "name" é obrigatória.

---
Mai/2020  
Guga Coder
