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
    
    let options = {}
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

export default {
  fetchPaper
}