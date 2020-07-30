<template lang="pug">
  v-app.home-view
    v-app-bar(
      app
      absolute
      color="primary"
      dark
      shrink-on-scroll
      prominent
      src="img/supermercado.jpg"
      fade-img-on-scroll
      scroll-target="#x-content"
      scroll-threshold="200"
    )
      template(
        v-slot:img="{ props }"
      )
        v-img(
          v-bind="props"
          gradient="to top right, rgba(103, 58, 183, .8), rgba(103, 58, 183, 0)"
        )

      v-app-bar-nav-icon

      v-toolbar-title
        span.font-weight-bold Processa
        span.font-weight-light App
          sup
            small.font-weight-light alfa

      v-spacer

      v-btn(
        icon
        @click="reload"
      )
        v-icon mdi-reload

      v-btn(
        icon
        :class="dark ? 'x-primary--pressed' : ''"
        @click="dark = !dark"
      )
        v-icon mdi-weather-night

      //-
        v-btn(
          icon
        )
          v-icon mdi-dots-vertical

    v-sheet#x-content.overflow-y-auto(
      :max-height="windowSize.y"
    )
      v-container(
        :style="`margin-top:128px; min-height: ${windowSize.y-128+1000}px;`"
      )
        v-col(
          class="text-center"
          cols="12"
        )
          span.title.font-weight-regular Bem-vindo ao &nbsp;
            span.font-weight-bold Processa
            span.font-weight-light App
              sup
                small.font-weight-light alfa

        v-col(
          class="text-center"
          cols="12"
        )
          span.font-weight-regular Estamos checando a sanidade do sistema
          br
          span.font-weight-regular Só deve levar alguns instantes...

        v-col(
          class="text-center"
          cols="12"
        )
          v-progress-linear(
            indeterminate
            color="primary"
          )

        v-col(
          class="text-center"
          cols="12"
        )
          v-btn(
            to="/!/App/Home/Index"
            color="primary"
          )
            | Abrir
</template>

<style lang="scss" scoped>
@use "sass:color";

.x-primary--pressed {
  background-color: rgba(149, 117, 205, .75)

  /*
  background-color: var(--v-primary-lighten2);
  background-color: var(--v-primary-base);
  background-color: rgba(var(--v-primary-lighten1), 1.0);
  */
}
</style>

<script>
import { BeforeInstallPromptEvent } from "vue-pwa-install";

export default {
  name: 'home-view',

  deferredPrompt: BeforeInstallPromptEvent,

  data: () => ({
    windowSize: {
      x: 0,
      y: 0
    }
  }),

  mounted () {
    this.onResize()
  },

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
    onResize () {
      this.windowSize = { x: window.innerWidth, y: window.innerHeight }
    },

    reload () {
      this.$router.go()
    }
  },

  promptInstall () {
    // Show the prompt:
    this.deferredPrompt.prompt()

    // Wait for the user to respond to the prompt:
    this.deferredPrompt.userChoice.then(choiceResult => {
      if (choiceResult.outcome === "accepted") {
        // User accepted the install prompt
      }

      this.deferredPrompt = null
    })
  },
}
</script>