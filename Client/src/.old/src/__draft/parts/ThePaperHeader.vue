<template lang="pug">
  div.the-paper-header
    v-app-bar(
      app
      color="primary"
      dark
      clipped-left
      clipped-right
    )
      v-btn(
        icon
        @click="$emit('menu')"
      )
        v-icon mdi-menu

      slot(
        name="left"
      )

      v-btn(
        text
        rounded
        large
        @click="$emit('menu')"
      )
        | {{ title }}

      v-spacer

      slot

      div(
        v-if="!identity"
      )
        //- Botao de menu para telas pequenas
        v-btn.d-flex.d-sm-none(
          icon
        )
          v-icon mdi-login-variant

        //- Botao de menu para telas pequenas
        v-btn.d-none.d-sm-flex(
          color="primary"
          large
          rounded
          depressed
        )
          | Entrar
          
          v-icon mdi-login-variant

      //- Menu do Usuário Logado
      v-menu(
        v-if="identity"
        :close-on-click="true"
        :close-on-content-click="true"
        :offset-x="false"
        :offset-y="true"
        dense
      )
        template(
          v-slot:activator="{ on, attrs }"
        )
          //- Botao de menu para telas pequenas
          v-btn.d-flex.d-sm-none(
            v-bind="attrs"
            v-on="on"
            icon
          )
            v-icon mdi-account-circle

          //- Botão de menu para telas grandes
          v-btn.d-none.d-sm-flex(
            v-bind="attrs"
            v-on="on"
            color="primary"
            large
            rounded
            depressed
          )
            | {{ identity.subject }}
            
            v-icon mdi-account-circle

        v-list
          v-list-item(
            disabled
          )
            v-list-item-title {{ identity.subject }}

          v-spacer

          v-list-item(
            @click="logout()"
          )
            v-list-item-title Sair
            
      slot(
        name="right"
      )
</template>

<style scoped>
</style>

<script>
//import Vue from 'vue'
import { mapState } from 'vuex'
import PaperBase from '../-PaperBase.vue'
import '@/helpers/StringHelper'

export default {
  extends: PaperBase,

  name: 'the-paper-header',

  data: () => ({
    loading: false,
  }),

  computed: {
    ...mapState('system', [
      'identity'
    ]),
  },

  watch: {
    'content.paper': 'awaitData',
  },

  methods: {
    logout () {
      this.identity = null
      this.$router.push('/')
      this.$router.go()
    },

    awaitData () {
      this.loading = !this.content.paper
    }
  }
}
</script>
