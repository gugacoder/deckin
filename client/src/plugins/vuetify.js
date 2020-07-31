import Vue from 'vue';
import Vuetify from 'vuetify/lib';
// import colors from 'vuetify/lib/util/colors'

Vue.use(Vuetify);

export default new Vuetify({
  theme: {
    options: {
      customProperties: true,
    },
    dark: true,
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
    // themes: {
    //   light: {
    //     primary: '#414751',
    //     secondary: colors.grey.darken1,
    //     accent: '#d41f3a', //colors.shades.black,
    //     error: colors.red.accent3,
    //   },
    //   dark: {
    //     primary: colors.deepPurple,
    //     secondary: colors.grey.darken1,
    //     accent: colors.shades.black,
    //     error: colors.red.accent3,
    //   },
    // },
  },
});
