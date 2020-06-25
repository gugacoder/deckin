import Vue from 'vue'
import Browser from '@/plugins/BrowserPlugin.js'

const state = {
  papers: {}
}

const mutations = {
  storePaper (state, payload) {
    let { href, paper } = payload
    Vue.set(state.papers, href, paper)
  },

  purgePaper (state, payload) {
    let { href } = payload
    Vue.delete(state.papers, href)
  }
}

const actions = {
  storePaper ({ commit }, payload) {
    return new Promise((resolve, reject) => {
      try {
        commit('storePaper', payload)
        resolve()
      } catch (error) {
        reject(error)
      }
    })
  },

  purgePaper ({ commit }, payload) {
    if (!payload.href) {
      payload = { href: payload }
    }
    return new Promise((resolve, reject) => {
      try {
        commit('purgePaper', payload)
        resolve()
      } catch (error) {
        reject(error)
      }
    })
  },

  async fetchPaper ({ dispatch }, payload) {
    if (!payload.href) {
      payload = { href: payload }
    }
    let { href, payload: content } = payload
    let paper = await Browser.fetch(href, content)
    await dispatch('storePaper', { href, paper })
  },

  async ensurePaper({ state, dispatch }, payload) {
    if (!payload.href) {
      payload = { href: payload }
    }
    if (!Object.prototype.hasOwnProperty.call(state.papers, payload.href)) {
      await dispatch('fetchPaper', payload)
    }
  }
}

const getters = {
  getPaper: (state) => (href) => {
    return state.papers[href]
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters
}