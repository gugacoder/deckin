import Vue from 'vue';
import Vuex from 'vuex';
import { entities } from './entities/index';
import { RootState } from './types';

Vue.use(Vuex);

export default new Vuex.Store<RootState>({
  state: {
  },
  modules: {
    entities,
  },
  mutations: {
  },
  actions: {
  },
});