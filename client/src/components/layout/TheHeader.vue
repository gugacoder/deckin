<template lang="pug">

  v-app-bar.the-paper-header(
    app
    color="primary"
    dark
    absolute
    :prominent="prominent"
    :shrink-on-scroll="prominent"
    :fade-img-on-scroll="prominent"
    :src="prominent ? '/img/header.jpg' : ''"
    scroll-target="#x-content"
    scroll-threshold="200"
  )
    template(
      v-slot:img="{ props }"
    )
      v-img(
        v-bind="props"
        :gradient="`to top right, var(--v-primary-${dark ? 'darken2' : 'base'}), rgba(65, 71, 81, 0)`"
      )

    v-app-bar-nav-icon(
      v-if="$listeners.menuClick"
      @click="$emit('menuClick')"
    )

    v-toolbar-title
      template(
        v-if="title"
      )
        | {{ title }}
      template(
        v-else
      )
        app-title

    slot(
      name="left"
    )

    v-spacer

    slot
          
    slot(
      name="right"
    )
</template>

<style scoped>
</style>

<script>
//import Vue from 'vue'
import AppTitle from '@/components/layout/AppTitle.vue'

export default {
  name: 'the-header',

  components: {
    AppTitle,
  },

  props: {
    title: {
      type: String,
      required: false
    },
    
    prominent: {
      type: Boolean,
      required: false
    },
  },

  data: () => ({
  }),

  computed: {
    dark: {
      get () {
        return this.$vuetify.theme.dark
      },

      set (value) {
        this.$vuetify.theme.dark = value
      }
    },
  },

  methods: {
  }
}
</script>