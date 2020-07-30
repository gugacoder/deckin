import lodash from 'lodash'
import '@/helpers/StringHelper.js'

export const unknownPaper = canonifyPaper({
  kind: 'fault',
  data: {
    fault: 'NotFound',
    reason: 'The requested path does not match any valid resource.'
  },
  links: [
    {
      rel: 'self',
      get href () { return window.location.pathname },
    }
  ]
})

export function getPaperId (paper) {
  let link = paper.links.filter(link => link.rel === 'self')[0]
  return link ? link.href : null
}

export function createPaperPromise (link) {
  return canonifyPaper({
    kind: 'promise',
    view: {
      ...(link.title ? { title: link.title } : {})
    },
    links: [
      {
        rel: 'self',
        href: link.href,
        ...(link.data ? { data: link.data } : {})
      }
    ]
  })
}

export function canonifyLink (link) {
  if (!link.href.includes('://')) {
    let href = link.href.replace(/[^/]+:[/]{2}[^/]+[/]/, '/')
    lodash.merge(link, { href })
  }
}

export function canonifyPaper (paper) {
  let index = 0;

  let source = paper
  let target = {
    kind: null,
    rel: null,
    meta: null,
    data: null,
    view: null,
    fields: null,
    actions: null,
    embedded: null,
    links: null,
    getLink (rel) {
      return this.links.filter(link => link.rel == rel)[0]
    }
  }

  // Kind, contém a identificação dos tipos representados pela entidade.
  //
  target.kind = source.kind || 'data'
  
  // Kind, contém a identificação dos tipos representados pela entidade.
  //
  target.rel = source.rel
  
  // Meta, contém instruções diretas para o motor do Paper.
  //
  target.meta = source.meta || {}

  // Data, contém os dados de usuário associados com a entidade.
  //
  target.data = source.data || {}
  // Recursivamente convertendo datas...
  // FIXME: deveria ser recursivo
  Object.keys(target.data).forEach(key => {
    let value = target.data[key]
    if (typeof value === 'string' && value.match(/[0-9/]+T[0-9:]/)) {
      target.data[key] = new Date(value)
    }
  })

  // View, contém instruções de rederização do componente.
  //
  target.view = source.view || {}
  if (target.kind === 'paper') {
    if (!target.view.design) target.view.design = 'data'
  }

  // Links, coleção dos links relacionados à entidade.
  //
  target.links = source.links || []
  // Caso Fields seja representado como um objeto o convertemos para
  // vetor definindo seu nome para uma propriedade de dados.
  // Isto:
  //    foo: {}
  // Se torna:
  //    {
  //      links: [
  //        {
  //          rel: 'foo'
  //        }
  //      ]
  //    }
  if (!Array.isArray(target.links)) {
    target.links = Object.keys(source.links).map(linkName => {
      let link = source.links[linkName]
      if (typeof link === 'string') {
        link = { href: link }
      }
      let properties = { rel: linkName }
      return lodash.merge({}, properties, link)
    })
  }
  // Tornando HREF relativo
  target.links.forEach(link => canonifyLink(link))

  // Fields, contém definições sobre os campos de dados, contidos em Data.
  //
  target.fields = source.fields || []
  // Caso Fields seja representado como um objeto o convertemos para
  // vetor definindo seu nome para uma propriedade de dados.
  // Isto:
  //    foo: {}
  // Se torna:
  //    {
  //      fields: [
  //        {
  //          name: 'foo'
  //        }
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
    field.view = field.view || {}

    // FIXME: Decidir onde a propriedade NAME deve ser mantida
    field.data.name = field.data.name || field.view.name
    field.view.name = field.view.name || field.data.name

    field.view.name = field.data.name || `_field${++index}`
    field.view.title = field.view.title || field.data.name.toProperCase()
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
    return canonifyPaper(field)
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
  //      actions: [
  //        {
  //          name: 'foo'
  //        }
  //      ]
  //    }
  if (!Array.isArray(target.actions)) {
    target.actions = Object.keys(source.actions).map(name => {
      let action = source.actions[name]
      let title = (action.view || {}).title
      let properties = {
        view: {
          name,
          title
        }
      }
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
    
    action = canonifyPaper(action)

    if (action.view.name === 'filter') {
      if (!action.view.title) {
        action.view.title = 'Filtro'
      }

      let link = action.links.filter(link => link.rel === 'action')[0]
      if (!link) {
        link = action.links.filter(link => link.rel === 'self')[0]
            || target.links.filter(link => link.rel === 'action')[0]
            || target.links.filter(link => link.rel === 'self')[0];
          
        action.links.push(link)
      }

      if (!link.title) {
        link.title = "Filtrar"
      }

    } else {
      if (!action.view.title) {
        action.view.title = name.toProperCase()
      }
    }

    return action
  })

  // Embedded, coleção de entidades adicionais entregues com a entidade
  //
  target.embedded = (source.embedded || []).map(entity => {
    return canonifyPaper(entity)
  })

  //
  // Pós-processamentos
  //

  // Corrigindo a falta de campos em designs como Grid
  if (target.kind === 'paper') {
    if (target.view.design === 'grid' && target.fields.length === 0) {
      let firstRecord = target.embedded[0]
      if (firstRecord) {
        target.fields = Object.keys(firstRecord.data).map(key => {
          return canonifyPaper({
            kind: 'header',
            data: {
              name: key
            },
            view: {
              title: key.toProperCase(),
              hidden: key.startsWith('_')
            }
          })
        })
      }
    }
  }

  return target
}

export default {
  unknownPaper,
  canonifyLink,
  canonifyPaper,
  getPaperId,
  createPaperPromise
}