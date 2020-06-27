<template lang="pug">
  div(
    class="paper-view"
  )
    v-app-bar(
      app
      dense
      color="white"
      elevate-on-scroll
    )
      v-app-bar-nav-icon

      v-toolbar-title
        span.font-weight-bold Paper
        span.font-weight-thin Alfa

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

      v-btn(
        icon
        @click="$router.push('/!/Keep.Paper/Home/Index')"
      )
        v-icon mdi-magnify

      v-btn(
        icon
        @click="$router.push('/!/My/Paper/Index')"
      )
        v-icon mdi-magnify

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

      v-btn(
        icon
      )
        v-icon mdi-dots-vertical

    v-container(
      id="paper-content"
    )
      //- :type= info success warning error
      v-alert(
        v-show="content.alert.message"
        :type="content.alert.type"
        dense
        text
        dismissible
        border="left"
        elevation="1"
        transition="fade-transition"
      )
        span(
          class="font-weight-medium"
        )
          | {{ content.alert.message }}
        
        br

        span(
          class="font-weight-light"
        )
          | {{ content.alert.detail }}
          
      template(
        v-if="paper"
      )
      
        component(
          :is="paperComponent"
          v-bind="paperComponentProperties"
        )

      br
      
      v-expansion-panels(
        flat
      )
        v-expansion-panel
          v-expansion-panel-header Raw data

          v-expansion-panel-content
            pre(v-if="paper") {{ paper }}
            small(v-else) ( Waiting for data... )

</template>

<script>
import Vue from 'vue'
import { unknownPaper } from '@/helpers/PaperHelper.js'
import '@/helpers/StringHelper.js'

export default {
  // Strategy: Fetching After Navigation

  name: 'paper-view',

  props:
  {
    catalogName: {
      type: String,
      required: true
    },
    paperName: {
      type: String,
      required: true
    },
    actionName: {
      type: String,
      required: true
    },
    actionKeys: {
      type: String,
      required: false
    }
  },

  components: {
  },

  data: () => ({
    content: {
      paper: null,
      alert: {
        type: null,
        message: null,
        detail: null,
        fault: null
      },
    },
    loading: false,
  }),

  computed: {
    actionArgs () {
      return (this.paperKeys || '').split(';')
    },
    
    identity () {
      return this.$identity
    },

    paper () {
      return this.content.paper
    },

    title () {
      return this.paper && this.paper.view.title
    },

    paperComponent () {
      let kind = (this.paper ? this.paper.kind : 'unknown').toHyphenCase()
      let name = `${kind}-paper`
      if (!Vue.options.components[name]) {
        name = 'unknown-paper'
      }
      return name;
    },

    paperComponentProperties () {
      return {
        catalogName: this.catalogName,
        paperName: this.paperName,
        actionName: this.actionName,
        actionKeys: this.actionKeys,
        content: this.content
      }
    },
  },

  created () {
    this.awaitData();
    this.fetchData();
  },

  watch: {
//    '$route': 'fetchData',
    paper: 'awaitData',
  },

  methods: {
    setPaper (value) {
      this.content.paper = value
    },

    async fetchData () {
      let self = (this.paper ? this.paper.getLink('self') : null) ||
          { href: this.$href(this) }

      let { href, data } = self
      let paper = await this.$fetch(href, data) || unknownPaper

      let path = this.$routeFor(href)
      if (path !== this.$route.path) {
        this.$router.push(path)
      }

      this.setPaper(paper)
    },

    awaitData () {
      this.content.alert = {}
      this.loading = !this.paper
    },

    logout () {
      this.$identity = null
      this.$router.push('/')
      this.$router.go()
    }
  }
}
</script>
