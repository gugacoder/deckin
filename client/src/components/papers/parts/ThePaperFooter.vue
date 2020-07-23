<template lang="pug">
  v-footer(
    app
    fixed
    elevation="4"
    color="white"
  )
    //- Exibir código fonte
    v-dialog(
      v-model="showSourceCode"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    )
      template(
        v-slot:activator="{ on, attrs }"
      )
        v-btn(
          v-bind="attrs"
          v-on="on"
          icon
          small
        )
          v-icon(
            small
            color="secondary lighten-5"
          )
            | mdi-code-json

      v-card
        v-toolbar(
          dark
          color="secondary lighten-3"
          dense
        )
          v-btn(
            icon
            dark
            @click="showSourceCode = false"
          )
            v-icon mdi-close
        
          v-icon code-json

          v-toolbar-title {{ $route.path }}
        
        v-list(
          three-line
          subheader
        )
          pre(
            v-if="content.paper"
          )
            | {{ content.paper }}

          small(
            v-else
          )
            | ( Aguardando dados... )
    
    v-btn(
      depressed
      rounded
      text
      small
      href="http://keepcoding.net"
      target="keepcoding.net"
    )
      small.font-weight-thin © 2020 KeepCoding™

    small
      slot(
        name="left"
      )
      
    v-spacer

    small
      slot

    small
      slot(
        name="right"
      )

</template>

<script>
//import Vue from 'vue'
import { mapState } from 'vuex'
import PaperBase from '../-PaperBase.vue'
import '@/helpers/StringHelper'

export default {
  extends: PaperBase,

  name: 'the-paper-footer',

  data: () => ({
    showSourceCode: false
  }),

  computed: {
    ...mapState('system', [
      'identity'
    ]),
  },
}
</script>