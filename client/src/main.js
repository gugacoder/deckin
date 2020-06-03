import Vue from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import store from './store'
import vuetify from './plugins/vuetify';

import DataPaper from './components/papers/DataPaper.vue';
import InvalidPaper from  './components/papers/InvalidPaper.vue';

Vue.config.productionTip = false

// Global components
Vue.component(DataPaper.name, DataPaper);
Vue.component(InvalidPaper.name, InvalidPaper);

new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount('#app')
