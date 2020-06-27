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

      v-toolbar-title {{ title }}
      
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
        v-if="!!identity"
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
        v-show="alert.message"
        :type="alert.type"
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
          | {{ alert.message }}
        
        br

        span(
          class="font-weight-light"
        )
          | {{ alert.detail }}
          
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
import { createNamespacedHelpers } from 'vuex'
import { createPaperPromise, unknownPaper } from '@/helpers/PaperHelper.js'
import '@/helpers/StringHelper.js'
import { API_PREFIX } from '@/plugins/BrowserPlugin.js'

const { mapActions, mapGetters } = createNamespacedHelpers('paper')

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
    loading: false,
    alert: {
      type: null,
      message: null,
      detail: null,
      fault: null
    },
    paper: null
  }),

  computed: {
    ...mapGetters([
      'getPaper'
    ]),

    identity () {
      return this.$identity
    },

    href () {
      return this.$href(this)
    },

    title () {
      return (this.paper ? this.paper.view.title : null) || 'Unnamed'
    },

    paperArgs () {
      return (this.paperKeys || '').split(';')
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
        paper: this.paper || unknownPaper
      }
    },
  },

  created () {
    this.awaitData();
    this.fetchData();
  },

  watch: {
    '$route': 'fetchData',
    paper: 'awaitData'
  },

  methods: {
    ...mapActions([
      'storePaper',
      'purgePaper',
      'fetchPaper',
      'ensurePaper',
    ]),

    awaitData() {
      this.alert = {}
      this.loading = !this.paper
    },

    async fetchData () {
      let paper

      paper = this.getPaper(this.href)
      if (paper && paper.kind === 'promise') {
        // Resolvendo promessa...
        let selfLink = paper.links.filter(link => link.rel === 'self')[0]
        await this.purgePaper(selfLink.href)
        await this.fetchPaper({ href: selfLink.href, payload: selfLink.data })
      }

      await this.ensurePaper(this.href)
      paper = this.getPaper(this.href) || unknownPaper
      await this.purgePaper(this.href)

      // Validando possível redirecionamento...
      //
      let forwardLink
      forwardLink = paper.links.filter(link => link.rel === 'forward')[0]
      if (!forwardLink) {
        let selfLink = paper.links.filter(link => link.rel === 'self')[0]
        if (selfLink && selfLink.href !== this.href) {
          forwardLink = selfLink
        }
      }
      if (forwardLink) {
        let href = forwardLink.href
        let route = forwardLink.href.replace(API_PREFIX, '/!')
        let promisePaper = createPaperPromise(forwardLink)
        await this.storePaper({ href, paper: promisePaper })
        this.$router.push(route)
        return
      }

      this.paper = paper
    },

    logout () {
      this.$identity = null
      this.$router.go()
    }
  }
}
</script>
