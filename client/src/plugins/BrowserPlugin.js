import Vue from 'vue'
import { canonifyPaper, unknownPaper } from '@/helpers/PaperHelper.js'
import '@/helpers/StorageHelper.js'
import store from '@/store'

const API_PREFIX = '/Api/1/Papers'

function href (catalogName, paperName, actionName, actionKeys) {
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
}

function routeFor (href) {
  return href.replace(API_PREFIX, '/!')
}

function requestData (href, payload, identity) {
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

function getIdentity() {
  return store.state.system.identity
}

function setIdentity(value) {
  store.state.system.identity = value
}

async function request (href, payload, options, inspector) {
  let paper
  let skipRedirection = options && options.skipRedirection
  let link = { href, data: payload }

  do {
    let { href, data } = link
    paper = await requestData(href, data, getIdentity()) || unknownPaper

    // Inspecting data...
    if (inspector) {
      inspector(paper)
    }
    
    // Applying metas...
    if (paper.meta.identity) {
      setIdentity(paper.meta.identity)
    }

    link = paper.getLink('forward')
  } while (!skipRedirection && link)

  return paper
}

const Browser = Vue.observable({
  API_PREFIX,
/*
  get identity() {
    return sessionStorage.getObject('identity')
  },
  set identity(value) {
    sessionStorage.setObject('identity', value)
  },
*/
  href,
  routeFor,
  request,
  install
})

function install (Vue /*, options */) {
  Vue.prototype.$browser = Browser
  //Vue.prototype.$request = request
  //Vue.prototype.$href = href
  //Vue.prototype.$routeFor = routeFor
/*
  Object.defineProperty(Vue.prototype, '$identity', {
    get: () => Browser.identity,
    set: (value) => Browser.identity = value
  })
  */
}

export { API_PREFIX }
export default Browser
