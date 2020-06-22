import lodash from 'lodash'

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

  // Kind, contém a identificação dos tipos representados pela entidade.
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
      let properties = { data: { name: fieldName } }
      return lodash.merge({}, field, properties)
    })
  }
  // Aplicando sanitizações
  index = 0
  target.fields = target.fields.map(field => {
    field.kind = field.kind || 'info'
    // FIXME: O nome deveria ser checado contra conflito de nome
    field.data = field.data || {}
    field.data.name = field.data.name || `_field${++index}`
    // Se um valor não é indicado diretamente então criamos uma função
    // de referência para a propriedade de mesmo nome na coleção de dados.
    if (!field.data.value) {
      if (!target.data[field.data.name]) {
        // Se não existe uma propriedade correspondente na coleção de dados
        // criamos uma valendo nulo.
        target.data[field.data.name] = null
      }
      Object.defineProperty(field.data, 'value', {
        get () {
          return target.data[field.data.name]
        },
        set (value) {
          target.data[field.data.name] = value
        }
      })
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
      return lodash.merge({}, properties, action)
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
      return lodash.merge({}, properties, link)
    })
  }

  return target
}

export function isKindOf(entity, kind) {
  return entity.kind.filter(x => x === kind).length > 0
}

export default {
  sanitizeEntity,
  isKindOf
}