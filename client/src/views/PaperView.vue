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

      router-link.title(
        to="/"
      )
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

      //-
        v-btn(
          icon
          @click="$router.push('/!/Keep.Paper/Home/Index')"
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
      fluid
    )
      //- :type can be either info, success, warning or error
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
        v-if="content.paper"
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
            pre(v-if="content.paper") {{ content.paper }}
            small(v-else) ( Waiting for data... )

</template>

<style scoped>
.title {
  text-decoration: none;
  color: darkslategray;
}
.title:hover {
  color: slategray;
}
</style>

<script>
import Vue from 'vue'
import { mapState } from 'vuex'
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
    preFetchedPaper: null,
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
    ...mapState('system', [
      'identity'
    ]),

    href () {
      return this.$browser.href(this)
    },

    actionArgs () {
      return (this.paperKeys || '').split(';')
    },
    
    title () {
      let paper = this.content.paper
      return paper && paper.view.title
    },

    paperComponent () {
      let paper = this.content.paper
      let design = (paper ? paper.view.design : 'unknown').toHyphenCase()
      let name = `${design}-paper`
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
    '$route': 'fetchData',
    'content.paper': 'awaitData',
  },

  methods: {
    async fetchData () {
      let targetPaper = this.preFetchedPaper

      if (!targetPaper) {
        let href = this.href
        let data = this.$route.query
        targetPaper = await this.$browser.request(href, data) || unknownPaper
      }

      let targetSelf = targetPaper.getLink('self') || { href: this.href }
      let targetPath = this.$browser.routeFor(targetSelf.href)
      if (targetPath !== this.$route.path) {
        this.fetchedPaper = targetPaper
        this.$router.push(targetPath)
        return
      }

      this.preFetchedPaper = null
      this.content.paper = targetPaper
    },

    awaitData () {
      this.content.alert = {}
      this.loading = !this.content.paper
    },

    logout () {
      this.identity = null
      this.$router.push('/')
      this.$router.go()
    }
  }
}
</script>
