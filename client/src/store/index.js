import Vue from 'vue'
import Vuex from 'vuex'
import modules from './modules'
import VuexPersist from 'vuex-persist'
import { moduleNames } from '@/store/modules'

Vue.use(Vuex)

const allModulesLocalPersist = new VuexPersist({
  key: 'Keep.Paper',
  storage: window.localStorage,
  modules: moduleNames.filter(name => name !== 'system')
})

// propriedades de sistema salvas localmente
const systemModuleLocalPersist = new VuexPersist({
  key: 'Keep.Paper',
  storage: window.localStorage,
  reducer (state) {
    let system = { ...state.system }
    delete system['identity'];
    return { system }
  }
})

// propriedades de sistema salvas na sessao apenas
const systemModuleSessionPersist = new VuexPersist({
  key: 'Keep.Paper',
  storage: window.sessionStorage,
  reducer (state) {
    return {
      system: {
        identity: state.system.identity
      }
    }
  }
})

// console.log(
//   systemModuleLocalPersist.plugin,
//   systemModuleSessionPersist.plugin
// )

export default new Vuex.Store({
  modules,
  plugins: [
    allModulesLocalPersist.plugin,
    systemModuleLocalPersist.plugin,
    systemModuleSessionPersist.plugin,
  ]
})
