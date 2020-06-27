import Vue from 'vue'
import { canonifyPaper } from '@/helpers/PaperHelper.js'
import '@/helpers/StorageHelper.js'

export const API_PREFIX = '/Api/1/Papers'

const BrowserPlugin = Vue.observable({
  API_PREFIX,

  get identity() {
    return sessionStorage.getObject('identity')
  },

  set identity(value) {
    sessionStorage.setObject('identity', value)
  },

  install (Vue /*, options */) {
    Vue.prototype.$browser = this
    Vue.prototype.$href = this.href
    Vue.prototype.$fetch = this.fetch

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

  fetch (href, payload) {
    return new Promise((resolve, reject) => {
      let options = {
        headers: new Headers()
      }
      
      if (this.identity && this.identity.token) {
        options.headers.append('Authorization', `Token ${this.identity.token}`)
      }
  
      options.method = 'post'
      options.body = JSON.stringify(payload || {})
  
      fetch(href, options)
        .then(response => response.json())
        .then(entity => canonifyPaper(entity))
        .then(entity => resolve(entity))
        .catch(error => reject(error))
    })
  },
})

export default BrowserPlugin
