<template lang="pug">
  v-navigation-drawer.the-app-user-menu(
    app
    right
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
        v-list-item-content.text-no-wrap
          app-title

        v-list-item-action
          v-btn(
            icon
            @click.stop="e => $emit('input', e.target.value)"
          )
            v-icon mdi-close

      v-list-item(
        dense
        style="background-color: rgba(0,0,0,.05);"
      )
        v-list-item-content.text-no-wrap
          template
            span.font-weight-light
              span Usuário

            span.font-weight-medium(
              v-if="identity"
            )
              | {{ identity.subject || 'Desconhecido' }}

            span.font-weight-medium(
              v-if="!identity"
            )
              | Nenhum usuário logado.

      slot(
        name="before"
      )

      slot

      v-list-item(
        link
        @click="toggleDark(!dark)"
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

      v-list-item(
        v-if="identity"
        link
        @click="logout()"
      )
        v-list-item-icon
          v-icon
            | mdi-logout-variant
          
        v-list-item-title Encerrar a sessão de usuário

      slot(
        name="after"
      )
</template>

<script>
import { mapState, mapActions } from 'vuex'
import AppTitle from '@/components/layout/AppTitle.vue'

export default {
  name: 'the-app-user-menu',

  components: {
    AppTitle
  },

  props: {
    value: {
      type: Boolean,
    },
  },

  data: () => ({
  }),

  computed: {
    ...mapState('system', [
      'identity',
      'dark',
    ]),
  },

  methods: {
    ...mapActions('system', [
      'setIdentity',
      'setDark',
    ]),

    toggleDark () {
      let currentState = this.$vuetify.theme.dark

      this.$vuetify.theme.dark = !currentState
      this.setDark(!currentState)
    },

    logout () {
      this.setIdentity(null)
      this.$router.push('/Home')
      this.$router.go()
    },
  },
}
</script>