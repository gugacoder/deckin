import Vue from 'vue'
import Vuex from 'vuex'
import { RootState } from '@/typings'
import { initialState } from '@/store/initial-state'

Vue.use(Vuex)

export default new Vuex.Store<RootState>({
  state: initialState,
  modules: {},
  mutations: {},  
  actions: {},
});