import { sanitizeEntity } from '@/helpers/EntityOperations.js'

export function fetchPaper(paper, payload) {
  return new Promise((resolve, reject) => {
    let tokens

    if (typeof paper === 'string') {
      // Formato: "/!/Catalog/Paper/Actions/Keys...
      let parts = paper.split('/').filter(x => x !== '').slice(1)
      tokens = [
        // catalogName
        parts.shift(),
        // paperName
        parts.shift(),
        // paperAction
        parts.shift(),
        // paperKeys
        ...parts
      ]
    } else {
      tokens = [
        paper.catalog || paper.catalogName,
        paper.name || paper.paperName,
        paper.action || paper.paperAction,
        ...(paper.keys || paper.paperKeys)
      ]
    }

    let path = `/!/${tokens.join('/')}`
    
    let options = {
      headers: new Headers()
    }
    
    let identity = sessionStorage.getItem('identity')
    if (identity && identity.token) {
      options.headers.append('Authorization', `Token ${identity.token}`)
    }

    if (payload) {
      options.method = 'post'
      options.body = JSON.stringify(payload)
    }

    fetch(path, options)
      .then(response => response.json())
      .then(data => sanitizeEntity(data))
      .then(data => resolve(data))
      .catch(error => reject(error))
  })
}

export function handlePaper(paper,
  showValidation, // func(message, field, data, paper)
  openPaper,      // func(paper)
  redirect        // func(href, paper)
) {
  // Processando metas
  if (paper.meta && paper.meta.identity) {
    sessionStorage.setItem('identity', paper.meta.identity)
  }

  // Processando validacoes
  if (paper.kind === 'validation' && paper.data && showValidation) {
    let field = paper.data.field
    let message = paper.data.message
    showValidation(message, field, paper.data, paper)
  }

  if (paper.meta && paper.meta.go && redirect) {
    var link = paper.links.filter(link => link.rel === paper.meta.go)[0];
    if (link && link.href) {
      var path = link.href.split('!/')[1];
      var href = `/Papers/${path}`;
      console.log({ redirect: href });
      redirect(href, paper)
    }
  }

  if (openPaper) {
    openPaper(paper)
  }
}

export default {
  fetchPaper
}