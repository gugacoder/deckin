<template lang="pug">
  div(
    class="paper"
  )
    v-app-bar(
      app
      dense
      color="white"
      elevate-on-scroll
    )
      v-app-bar-nav-icon

      v-toolbar-title
        strong Director
        | Alfa
        
        span(
          v-show="title"
        )
          | &nbsp; - {{ title }}
      
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
      )
        v-icon mdi-magnify

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
            pre {{ paper }}
</template>

<script>
import Vue from 'vue'
import '@/helpers/StringOperations.js'
import { fetchPaper, handlePaper } from '@/services/PaperService.js'

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
    title () {
      return !this.paper || this.paper.view.title
    },
    
    paperArgs () {
      return (this.paperKeys || '').split(';')
    },

    paperComponent () {
      var pascalName = this.paper.kind.toHyphenCase()
      var componentName = `${pascalName}-paper`
      if (!Vue.options.components[componentName]) {
        componentName = 'invalid-paper'
      }
      return componentName;
    },

    paperComponentProperties () {
      return {
        catalogName: this.catalogName,
        paperName: this.paperName,
        paperAction: this.paperAction,
        paperKeys: this.paperKeys,
        paper: this.paper
      };
    }
  },

  created () {
    this.fetchData();
  },

  watch: {
    '$route': 'fetchData'
  },

  methods: {
    fetchData () {
      this.alert = {}
      this.paper = null
      this.loading = true
      fetchPaper(this)
        .then(paper => this.showPaper(paper))
        .catch(error => this.alert = {
          type: 'error',
          message: 'O servidor não respondeu como esperado.',
          detail: `Certifique-se de que o servidor esteja operando normalmente e
              disponível nesta mesma rede.`,
          fault: error
        })
        .finally(() => {
          this.loading = false
        })
    },

    showPaper (paper) {
      handlePaper(paper, null, this.setPaper, this.redirectPaper)
    },

    setPaper (paper) {
      this.paper = paper
    },

    redirectPaper (href) {
      this.$router.push(href);
    }
  }
}
</script>
