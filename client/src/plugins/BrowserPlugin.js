import Vue from 'vue'
import { canonifyPaper, unknownPaper } from '@/helpers/PaperHelper.js'
import '@/helpers/StorageHelper.js'

export const API_PREFIX = '/Api/1/Papers'

function _fetch (href, payload, identity) {
  return new Promise((resolve, reject) => {
    let options = {
      headers: new Headers()
    }
    
    if (identity && identity.token) {
      options.headers.append('Authorization', `Token ${identity.token}`)
    }

    options.method = 'post'
    options.body = JSON.stringify(payload || {})

    fetch(href, options)
      .then(response => response.json())
      .then(entity => canonifyPaper(entity))
      .then(entity => resolve(entity))
      .catch(error => reject(error))
  })
}

export default Vue.observable({
  API_PREFIX,

  get identity() {
    return sessionStorage.getObject('identity')
  },

  set identity(value) {
    sessionStorage.setObject('identity', value)
  },

  install (Vue /*, options */) {
    Vue.prototype.$browser = this
    Vue.prototype.$fetch = this.fetch
    Vue.prototype.$href = this.href
    Vue.prototype.$routeFor = this.routeFor

    Object.defineProperty(Vue.prototype, '$identity', {
      get: () => this.identity,
      set: (value) => this.identity = value
    })
  },

  href (catalogName, paperName, actionName, actionKeys) {
    if (!(catalogName instanceof String)) {
      let options = catalogName
      catalogName = options.catalogName
      paperName = options.paperName
      actionName = options.actionName
      actionKeys = options.actionKeys
    }

    if (Array.isArray(actionKeys)) {
      actionKeys = actionKeys.join(';')
    }

    let tokens = [
      catalogName,
      paperName,
      actionName,
      ...(actionKeys ? [ actionKeys ] : [])
    ]

    let path = tokens.join('/')
    return `${API_PREFIX}/${path}`
  },

  routeFor (href) {
    return href.replace(API_PREFIX, '/!')
  },

  async fetch (href, payload, options, inspector) {
    let paper
    let skipRedirection = options && options.skipRedirection
    let link = { href, data: payload }

    do {
      let { href, data } = link
      paper = await _fetch(href, data, this.$identity) || unknownPaper

      // Inspecting data...
      if (inspector) {
        inspector(paper)
      }
      
      // Applying metas...
      if (paper.meta.identity) {
        this.$identity = paper.meta.identity
      }

      link = paper.getLink('forward')
    } while (!skipRedirection && link)

    return paper
  },
})
