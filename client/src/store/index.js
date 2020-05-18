import Vue from 'vue'
import Vuex from 'vuex'
import account from './modules/account.js'

Vue.use(Vuex)

const isDevMode = process.env.NODE_ENV !== 'production'

export default new Vuex.Store({
  strict: isDevMode,
  modules: {
    account
  }
})
