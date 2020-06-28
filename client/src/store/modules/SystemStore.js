import Vue from 'vue'

const state = {
  identity: null
}

const mutations = {
  setIdentity (state, identity) {
    state.identity = identity
  }
}

const actions = {
  setIdentity ({ commit }, identity) {
    return new Promise((resolve, reject) => {
      try {
        commit('setIdentity', identity)
        resolve()
      } catch (error) {
        reject(error)
      }
    })
  }
}

const getters = {
}

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters
}