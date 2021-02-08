import Vue from 'vue'
import Vuex from 'vuex'
import { Module } from 'vuex';
import { getters } from './getters';
import { actions } from './actions';
import { mutations } from './mutations';
import { RootState } from '../types';
import { Entity } from './types';

export const state: Entity = {
  self: ''
};

const namespaced: boolean = true;

Vue.use(Vuex)

export const entities: Module<Entity, RootState> = {
  namespaced,
  state,
  getters,
  actions,
  mutations,
};
