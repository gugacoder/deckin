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
    props: {
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
    props: null,
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

  // props, contém instruções de rederização do componente.
  //
  target.props = source.props || {}

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
    field.kind = field.kind || 'field'
    // FIXME: O nome deveria ser checado contra conflito de nome
    field.data = field.data || {}
    field.props = field.props || {}

    field.props.name = field.props.name || `_field${++index}`
    field.props.title = field.props.title || field.props.name.toProperCase()
    // Se um valor não é indicado diretamente então criamos uma função
    // de referência para a propriedade de mesmo nome na coleção de dados.
    if (!field.data.value) {
      if (!target.data[field.props.name]) {
        // Se não existe uma propriedade correspondente na coleção de dados
        // criamos uma valendo nulo.
        target.data[field.props.name] = null
      }
      Object.defineProperty(field.data, 'value', {
        get () {
          return target.data[field.props.name]
        },
        set (value) {
          target.data[field.props.name] = value
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
      let title = (action.props || {}).title
      let properties = {
        props: {
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
    action.props = action.props || {}
    action.props.name = action.props.name || `_action${++index}`
    
    action = canonifyPaper(action)

    if (action.props.name === 'filter') {
      if (!action.props.title) {
        action.props.title = 'Filtro'
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
      if (!action.props.title) {
        action.props.title = name.toProperCase()
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
  if (target.kind === 'action') {
    if (target.props['@type'] === 'grid' && target.fields.length === 0) {
      let firstRecord = target.embedded[0]
      if (firstRecord) {
        target.fields = Object.keys(firstRecord.data).map(key => {
          return canonifyPaper({
            kind: 'header',
            data: {
              name: key
            },
            props: {
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