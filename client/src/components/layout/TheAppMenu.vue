<template lang="pug">
  v-navigation-drawer.the-app-menu(
    app
    :width="expanded ? 590 : 300"
    :value="value"
  )
    v-list
      v-list-item.text-no-wrap.pr-3(
        @click="$emit('input', $event.target.value)"
      )
        v-list-item-icon
          v-icon mdi-menu

        v-list-item-content.text-uppercase.font-weight-bold
          | Menu

        v-btn.mr-0(
          icon
          @click.stop="expanded = !expanded"
          right
        )
          v-icon {{ expanded ? 'mdi-unfold-less-vertical' : 'mdi-unfold-more-vertical' }}

      slot(
        name="before"
      )

      slot

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

        v-list-item-content.text-no-wrap
          | Usar um tema {{ dark ? 'claro' : 'escuro' }} 

      slot(
        name="after"
      )
</template>

<script>
export default {
  name: 'the-app-menu',

  props: {
    value: {
      type: Boolean,
      required: false,
    }
  },

  data: () => ({
    expanded: false,
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
}
</script>