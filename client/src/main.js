import Vue from 'vue'
import App from './App.vue'
import AppConfig from './AppConfig.js'
import './registerServiceWorker'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify'

// Vue PWA: https://laniewski.me/vue/pwa/offline/2019/01/04/creating-offline-first-vue-apps.html
import VueInstall from "vue-pwa-install";
import BrowserPlugin from '@/plugins/BrowserPlugin.js'

Vue.config.productionTip = false

Vue.use(BrowserPlugin)
Vue.use(VueInstall)

// Registrando componentes globais...
import '@/components'

Vue.prototype.$app = AppConfig

Object.defineProperty(Vue.prototype, '$isMobile', {
  get () {
    return this.$vuetify.breakpoint.xsOnly
  }
})

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
