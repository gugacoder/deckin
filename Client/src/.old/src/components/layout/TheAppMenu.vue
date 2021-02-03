<template lang="pug">
  v-navigation-drawer.the-app-menu(
    app
    style="z-index:1;"
    :width="$isMobile ? '100%' : 375"
    :value="value"
    @input="value => $emit('input', value)"
  )
    v-list.pt-0

      v-list-item(
        link
        dense
        style="background-color: rgba(0,0,0,.05);"
        @click.stop="e => $emit('input', e.target.value)"
      )
        v-icon mdi-menu

        v-list-item-content.text-no-wrap
          app-title

        v-list-item-action
          v-btn(
            icon
            @click.stop="e => $emit('input', e.target.value)"
          )
            v-icon mdi-close

      //- Título da página corrente
      v-list-item(
        v-if="caption"
        dense
        style="background-color: rgba(0,0,0,.05);"
      )
        v-list-item-content.text-no-wrap
          span.font-weight-light(
            v-if="catalog"
          )
            span {{ catalog }}

          span.font-weight-medium.text-no-wrap {{ caption }}

      slot(
        name="before"
      )

      slot

      //-
        v-list-item(
          link
        )
          v-list-item-icon
            v-icon
              | mdi-icon

          v-list-item-content.text-no-wrap
            | Text

      slot(
        name="after"
      )
</template>

<script>
import AppTitle from '@/components/layout/AppTitle.vue'

export default {
  name: 'the-app-menu',

  components: {
    AppTitle
  },

  props: {
    value: {
      type: Boolean,
    },
    
    catalog: {
      type: String,
    },
    
    caption: {
      type: String,
    },
  },

  data: () => ({
    busy: false,
  }),

  computed: {
    dark: {
      get () {
        return this.$vuetify.theme.dark
      },
      set (value) {
        this.$vuetify.theme.dark = value
        this.setDark(value)
      }
    },
  },

  methods: {
  }
}
</script>