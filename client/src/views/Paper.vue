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
import '@/helpers/StringExtensions.js'
import { sanitizeEntity } from "@/helpers/EntityOperations.js";

export default {
  // Strategy: Fetching After Navigation

  name: 'paper-view',

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
      if (this.paper === null) return null;
      return [
        this.paperName, 
        this.paperAction,
        ...this.paperKeys
      ].join(' / ');
    },
    
    catalogName () {
      var path = this.$route.params.pathMatch;
      return path.split('/')[0] ?? null;
    },
    
    paperName () {
      var path = this.$route.params.pathMatch;
      return path.split('/')[1] ?? null;
    },
    
    paperAction () {
      var path = this.$route.params.pathMatch;
      return path.split('/')[2] ?? null;
    },
    
    paperKeys () {
      var path = this.$route.params.pathMatch;
      return path.split('/').slice(3);
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

      var tokens = [
        this.catalogName,
        this.paperName,
        this.paperAction,
        ...this.paperKeys
      ]
      var path = `/!/${tokens.join('/')}`
      
      fetch(path)
        .then(response => response.json())
        .then(data => sanitizeEntity(data))
        .then(data => this.openPaper(data))
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

    openPaper (paper) {
      var meta = paper.meta;
      if (meta && meta.go) {
        var link = paper.links.filter(link => link.rel === meta.go)[0];
        if (link && link.href) {
          var path = link.href.split('!/')[1];
          var href = `/Papers/${path}`;
          console.log({ autoRedirectingTo: href });
          this.$router.push(href);
        }
      }

      this.paper = paper;
    }
  }
}
</script>
