<template lang="pug">
  div(
    class="paper-view"
  )
    v-system-bar(
      app
      color="white"
      v-if="true"
    )
      v-icon mdi-cart-outline

      v-hover(
        v-slot:default="{ hover }"
      )
        span.custom-title
          router-link(
            to="/"
          )
            span.font-weight-bold Mercado
            span.font-weight-light Logic &nbsp;
              small.font-weight-light Alfa

      v-spacer

      v-icon mdi-alarm

      span {{ currentDate }}

      v-icon

    v-container(
      id="paper-content"
      fluid
    )

      //- :type can be either info, success, warning or error
      v-alert(
        v-model="content.alert.valid"
        v-show="content.alert.message"
        :type="content.alert.type || 'info'"
        dense
        dismissible
        text
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
          v-bind="paperComponent"
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
.custom-title a {
  text-transform: uppercase;
  text-decoration: none;
  color: var(--v-secondary-base)
}

.custom-title a:hover {
  text-transform: uppercase;
  text-decoration: none;
  color: lighten( red, 50% );
}
</style>

<script>
import Vue from 'vue'
import { mapState } from 'vuex'
import moment from 'moment'
import { unknownPaper } from '@/helpers/PaperHelper'
import '@/helpers/StringHelper'

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
        valid: true,
        type: null,
        message: null,
        detail: null,
        fault: null
      },
    },

    currentDate: null,
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

      return {
        is: name,
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
    this.startTimer();
  },

  watch: {
    '$route': 'fetchData',
    'content.paper': 'awaitData',
  },

  methods: {
    startTimer() {
      if (!this.currentDate) {
        setInterval(() => {
          this.currentDate = moment().format('LLLL')
        }, 1000)
      }
    },

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
    },
  }
}
</script>
