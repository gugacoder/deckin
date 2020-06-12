export function sanitizeEntity (entity) {
  let index = 0;

  let source = entity
  let target = {
    kind: null,
    meta: null,
    data: null,
    view: null,
    fields: null,
    actions: null,
    embedded: null,
    links: null
  }

  // Kind, contém o tipo do componente vinculado.
  //
  target.kind = source.kind || 'data'

  // Meta, contém instruções diretas para o motor do Paper.
  //
  target.meta = source.meta || {}

  // Data, contém os dados de usuário associados com a entidade.
  //
  target.data = source.data || {}

  // View, contém instruções de rederização do componente.
  //
  target.view = source.view || {}

  // Fields, contém definições sobre os campos de dados, contidos em Data.
  //
  target.fields = source.fields || []
  // Caso Fields seja representado como um objeto o convertemos para
  // vetor definindo seu nome para uma propriedade de dados.
  // Isto:
  //    foo: {}
  // Se torna:
  //    {
  //      view: {
  //        name: "foo"
  //      }
  //    }
  if (!Array.isArray(target.fields)) {
    target.fields = Object.keys(source.fields).map(fieldName => {
      let field = source.fields[fieldName]
      let properties = { view: { name: fieldName } }
      return Object.assign({}, field, properties)
    })
  }
  // Aplicando sanitizações
  index = 0
  target.fields = target.fields.map(field => {
    field.kind = field.kind || 'info'
    // FIXME: O nome deveria ser checado contra conflito de nome
    field.view = field.view || {}
    field.view.name = field.view.name || `_field${++index}`
    // Se um valor não é indicado diretamente então criamos uma função
    // de referência para a propriedade de mesmo nome na coleção de dados.
    if (!field.view.value) {
      Object.defineProperty(field.view, 'value', {
        get: () => target.data[field.view.name],
        set: (value) => target.data[field.view.name] = value
      })
      // Se não existe uma propriedade correspondente na coleção de dados
      // criamos uma valendo nulo.
      target.data[field.view.name] = target.data[field.view.name] || null
    }
    return sanitizeEntity(field, target)
  })

  // Actions, coleção de ações aplicadas em cima da entidade
  //
  target.actions = source.actions || []
  // Caso Actions seja representado como um objeto o convertemos para
  // vetor definindo seu nome para uma propriedade de dados.
  // Isto:
  //    foo: {}
  // Se torna:
  //    {
  //      view: {
  //        name: "foo"
  //      }
  //    }
  if (!Array.isArray(target.actions)) {
    target.actions = Object.keys(source.actions).map(actionName => {
      let action = source.actions[actionName]
      let properties = { view: { name: actionName } }
      return Object.assign({}, properties, action)
    })
  }
  // Aplicando sanitizações
  index = 0
  target.actions = target.actions.map(action => {
    action.kind = action.kind || 'action'
    // FIXME: O nome deveria ser checado contra conflito de nome
    action.view = action.view || {}
    action.view.name = action.view.name || `_action${++index}`
    return sanitizeEntity(action, target)
  })

  // Embedded, coleção de entidades adicionais entregues com a entidade
  //
  target.embedded = (source.embedded || []).map(entity => {
    return sanitizeEntity(entity, target)
  })

  // Links, coleção dos links relacionados à entidade.
  //
  target.links = source.links || []
  // Caso Fields seja representado como um objeto o convertemos para
  // vetor definindo seu nome para uma propriedade de dados.
  // Isto:
  //    foo: {}
  // Se torna:
  //    {
  //      rel: "foo"
  //    }
  if (!Array.isArray(target.links)) {
    target.links = Object.keys(source.links).map(linkName => {
      let link = source.links[linkName]
      let properties = { rel: linkName }
      return Object.assign({}, properties, link)
    })
  }

  return target
}

export default {
  sanitizeEntity
}