import { sanitizeEntity } from '@/helpers/EntityOperations.js'

let protocol = window.location.protocol
let host = window.location.hostname
let port = window.location.port

// PORTA DO SERVIDOR DA API
port = 5000

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
        paper.catalogName,
        paper.paperName,
        paper.actionName,
        ...(paper.actionKeys ? [ paper.actionKeys ] : [])
      ]
    }

    let path = `${protocol}://${host}:${port}/Api/1/Entities/${tokens.join('/')}`
    
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

    console.log({ path });
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
  if (paper.meta.identity) {
    sessionStorage.setItem('identity', paper.meta.identity)
  }

  // Processando validacoes
  if (showValidation && paper.kind === 'validation') {
    let field = paper.data.field
    let message = paper.data.message
    showValidation(message, field, paper.data, paper)
  }

  // Checando link de redirecionamento
  var forwardLink = paper.links.filter(link => link.rel === 'forward')[0]
  if (redirect && forwardLink) {
    var path = forwardLink.href.split('/Api/1/Entities/')[1];
    var href = `/!/${path}`;
    console.log({ redirect: href });
    redirect(href, paper)
  }

  if (openPaper) {
    openPaper(paper)
  }
}

export default {
  fetchPaper
}