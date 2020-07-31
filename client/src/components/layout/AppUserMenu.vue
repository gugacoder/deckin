<template lang="pug">
  div.user-menu
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
        text
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

        v-slot(
          name="before"
        )

        v-slot

        v-list-item(
          @click="logout()"
        )
          v-list-item-title Sair

        v-slot(
          name="after"
        )
</template>

<script>
import { mapState } from 'vuex'

export default {
  name: 'user-menu',

  components: {
  },

  props: {
  },

  data: () => ({
  }),

  computed: {
    ...mapState('system', [
      'identity'
    ]),
  },

  methods: {
    logout () {
      this.identity = null
      this.$router.push('/')
      this.$router.go()
    },
  },
}
</script>