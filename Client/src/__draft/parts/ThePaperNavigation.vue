<template lang="pug">
  v-navigation-drawer.the-paper-navigation(
    v-model="visible"
    app
    :width="expanded ? 590 : 300"
  )
    v-list(
    )
      v-list-item.x-nowrap.pr-3(
        @click="visible = !visible"
      )
        v-list-item-icon
          v-icon mdi-menu

        v-list-item-content.x-title.font-weight-bold
          | Menu

        v-btn.mr-0(
          icon
          @click.stop="expanded = !expanded"
          right
        )
          v-icon {{ expanded ? 'mdi-unfold-less-vertical' : 'mdi-unfold-more-vertical' }}

      v-list-item(
        link
      )
        v-list-item-icon
          v-icon mdi-filter

        v-list-item-content
          | Filtro

      v-list-item(
        link
        @click="dark = !dark"
      )
        v-list-item-icon
          v-icon(
            v-show="!dark"
          )
            | mdi-weather-night

          v-icon(
            v-show="dark"
          )
            | mdi-weather-sunny

        v-list-item-content.x-nowrap
          | Usar um tema {{ dark ? 'claro' : 'escuro' }} 
</template>

<style scoped>
.x-nowrap {
  white-space: nowrap;
}

.x-title {
  text-transform: uppercase;
}

.x-btn-pressed {
  background-color: var(--v-primary-lighten1) !important;
}
</style>

<script>
import PaperBase from '../-PaperBase.vue'

export default {
  extends: PaperBase,

  name: 'the-paper-menu',

  props: {
    // Estrutura:
    // {
    //   visible: Boolean,
    //   expanded: Boolean
    // }
    menu: {
      type: Object,
      required: true
    }
  },

  data: () => ({
  }),

  computed: {
    visible: {
      get () {
        return this.menu.visible
      },
      set (value) {
        this.$set(this.menu, 'visible', value)
      }
    },

    expanded: {
      get () {
        return this.menu.expanded
      },
      set (value) {
        this.$set(this.menu, 'expanded', value)
      }
    },

    dark: {
      get () {
        return this.$vuetify.theme.dark
      },

      set (value) {
        this.$vuetify.theme.dark = value
      }
    }
  }
}
</script>