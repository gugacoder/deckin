<template lang="pug">
  div.the-paper-header
    v-app-bar(
      app
      color="primary"
      dark
      clipped-left
    )
      slot(
        name="left"
      )

      v-btn(
        rounded
        text
        @click="$emit('click')"
      )
        | {{ title }}

      v-spacer

      template(
        v-if="loading"
      )
        span(
          class="text--secondary"
        )
          | Carregando dados...

        v-progress-linear(
          :indeterminate="true"
          height="2"
          absolute
          bottom
        )

      slot

      v-menu(
        :close-on-click="true"
        :close-on-content-click="true"
        :offset-x="false"
        :offset-y="true"
        v-if="identity"
        dense
      )
        template(
          v-slot:activator="{ on, attrs }"
        )
          v-btn(
            icon
            v-bind="attrs"
            v-on="on"
          )
            v-icon mdi-account-circle

        v-list
          v-list-item(
            disabled
          )
            v-list-item-title {{ identity.subject }}

          v-list-item(
            @click="logout()"
          )
            v-list-item-title Sair do Sistema
      
      slot(
        name="right"
      )
</template>

<script>
//import Vue from 'vue'
import { mapState } from 'vuex'
import BasePaperPart from '../BasePaperPart'
import '@/helpers/StringHelper'

export default {
  extends: BasePaperPart,

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