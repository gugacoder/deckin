import Vue from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'

Vue.config.productionTip = false

//import browserPlugin from '@/plugins/browser-plugin.js'
//Vue.use(browserPlugin)
//console.log('Hi there')

// Registrandos componentes globais...
import '@/components'

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
