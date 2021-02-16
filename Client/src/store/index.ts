import Vue from 'vue'
import Vuex from 'vuex'
import { State } from '../typings'
import { InitialState } from './initialState'

Vue.use(Vuex)

export default new Vuex.Store<State>({
  state: InitialState,
  /*
  modules: {},
  mutations: {},  
  actions: {},
  */
});