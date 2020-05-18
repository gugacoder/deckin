import router from '@/router';
import account from '@/services/account.js'

const user = JSON.parse(localStorage.getItem('user'));

const state = {
  status: {
    loggedIn: true
  },
  user
}

//const delay = (delay, value) =>
//    new Promise(resolve => setTimeout(resolve, delay, value));

const getters = {
}

const actions = {
  login({ dispatch, commit }, { username, password }) {
    commit('loginRequest', { username });

    account.login(username, password)
      .then(
        user => {
          commit('loginSuccess', user);
          router.push('/');
        },
        error => {
          commit('loginFailure', error);
          dispatch('alert/error', error, { root: true });
        }
      );
  },
  logout({ commit }) {
    account.logout();
    commit('logout');
  }
}

const mutations = {
  loginRequest(state, user) {
    state.status = { loggingIn: true };
    state.user = user;
  },
  loginSuccess(state, user) {
    state.status = { loggedIn: true };
    state.user = user;
  },
  loginFailure(state) {
    state.status = {};
    state.user = null;
  },
  logout(state) {
    state.status = {};
    state.user = null;
  }
}

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations
}
