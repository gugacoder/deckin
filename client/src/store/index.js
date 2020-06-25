import Vue from 'vue'
import Vuex from 'vuex'
import PaperStore from './modules/PaperStore.js'

Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    paper: PaperStore
  }
})
