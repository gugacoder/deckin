const state = {
  identity: null,
  dark: false,
}

const mutations = {
  setIdentity (state, identity) {
    state.identity = identity
  },

  setDark (state, dark) {
    state.dark = dark
  },
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
  },

  setDark ({ commit }, dark) {
    return new Promise((resolve, reject) => {
      try {
        commit('setDark', dark)
        resolve()
      } catch (error) {
        reject(error)
      }
    })
  },
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