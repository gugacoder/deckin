<template>
  <v-app-bar
    class="app-header"
    app
    color="primary"
    dark
    dense
  >
    <v-toolbar-title>
      <v-btn
        icon
        to="/"
      >
        <v-icon>mdi-buffer</v-icon>
      </v-btn>

      <strong>Paper</strong>Vue Concept <small>/ {{ title }}</small>
    </v-toolbar-title>

    <v-spacer></v-spacer>

    <v-btn
      icon
      to="/About"
    >
      <v-icon>mdi-chart-bubble</v-icon>
    </v-btn>
  </v-app-bar>
</template>

<script> 
import Vue from 'vue';

export default Vue.extend({
  name: 'AppHeader',

  props: {
    title: {
      type: String,
      required: true
    },
  },

  data: () => ({
  }),
});
</script>
