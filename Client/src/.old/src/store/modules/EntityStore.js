import Vue from 'vue'

const state = {
}

const mutations = {
  save (state, { href, entity }) {
    entity.meta.storedAt = new Date()
    Vue.set(state, href, entity)
  },

  /*
  storeEntity (state, entity) {
    let self = entity.links.filter(link => link.rel === 'self')[1]
    if (!self)
      throw new TypeError(
        "Não é possível estocar uma entidade sem sua identificação de URI. (Missing self link.)")

    Vue.set(state, self.href, entity)
  },
  */
}

const actions = {
  get ({ commit }, href, payload) {
    return new Promise((resolve, reject) => {
      try {
        Vue.$browser.request(href, payload)
          .then(entity => {
            if (entity && entity.kind === 'action') {
              let members = dismember(href, entity)

              members.forEach(member => {
                commit('save', member)
              });

              let main = members[0]
              entity = main.entity
            }
          })
          .then(resolve)
          .catch(reject)
      } catch (error) {
        reject(error)
      }
    })
  },



  change ({ state, commit }) {
    let entity = state['/Api/1/Papers/Home']
    if (entity.data.id === 10) {
      commit('save', {
        data: {
          id: 20,
          name: 'Twenty'
        }
      })
    } else {
      commit('save', {
        data: {
          id: 10,
          name: 'Ten'
        }
      })
    }
  },
  /*
  storeEntity ({ commit }, entity) {
    return new Promise((resolve, reject) => {
      try {
        commit('storeEntity', entity)
        resolve()
      } catch (error) {
        reject(error)
      }
    })
  },

  fetchEntity ({ dispatch }, { href, renew }) {
    let entity = state[href]
    if (renew || !entity) {
      entity = await this.$browser.fetch(href)
      dispatch('storeEntity', entity)
    }
    return entity
  },
  */
}

const getters = {
}

const dismember = function (href, entity) {
  let entities = [ { href, entity } ]
  entity.embedded.forEach(child => {
    let self = child.links.filter(link => link.rel === 'self')[0]
    if (self && self.href) {
      // emancipando uma entidade
      let target = Object.assign({}, child)
      let targets = dismember(self.href, target)
      targets.forEach(x => entities.push(x));

      // criando uma referencia para a entidade
      let targetRef = { kind: child.kind, href: self.href }

      // tornando a entidade embarcada uma referencia
      Object.keys(child).forEach(function(key) { delete child[key]; });
      Object.assign(child, targetRef)
    }
  });
  return entities
}

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters
}