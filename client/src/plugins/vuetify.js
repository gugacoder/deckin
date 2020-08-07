import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import store from '@/store';
import '@mdi/font/css/materialdesignicons.css' // Ensure you are using css-loader

Vue.use(Vuetify)

export default new Vuetify({
  icons: {
    iconfont: 'mdi',
  },

  theme: {
    options: {
      customProperties: true,
    },

    dark: store.state.system.dark,

    themes: {
      light: {
        primary: '#515b7e',
        accent: '#d71f3b',
      },
      dark: {
        primary: '#7382b7', //'#8293ce',
        accent: '#d71f3b',
      },
    }
  },
});
