import Vue from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'
import BrowserPlugin from '@/plugins/BrowserPlugin.js'

Vue.config.productionTip = false

Vue.use(BrowserPlugin)

// Registrando componentes globais...
import '@/components'

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
