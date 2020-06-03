<template>
  <div
    class="paper"
  >
    <v-app-bar
      app
      dense
      color="white"
      elevate-on-scroll
    >    
      <v-app-bar-nav-icon></v-app-bar-nav-icon>

      <v-toolbar-title>
        <strong>Director</strong>Alfa
        
        <span v-show="title">-</span>
        
        {{title}}
      </v-toolbar-title>
      
      <v-spacer></v-spacer>

      <template
        v-if="loading"
      >
        <span
          class="text--secondary"
        >
          Carregando dados...
        </span>

        <v-progress-linear
          :indeterminate="true"
          height="2"
          absolute
          bottom
        ></v-progress-linear>
      </template>

      <v-btn icon>
        <v-icon>mdi-magnify</v-icon>
      </v-btn>

      <v-btn icon>
        <v-icon>mdi-dots-vertical</v-icon>
      </v-btn>
    </v-app-bar>

    <v-container
      id="paper-content"
    >
      <!-- info success warning error -->
      <v-alert
        v-show="alert.message"
        :type="alert.type"
        dense
        text
        dismissible
        border="left"
        elevation="1"
        transition="fade-transition"
      >
        <span
          class="font-weight-medium"
        >
          {{alert.message}}
        </span>
        
        <br>

        <span
          class="font-weight-light"
        >
          {{alert.detail}}          
        </span>
      </v-alert>

      <template v-if="paper">
        <component :is="paperComponent" v-bind="paperComponentProperties">
        </component>
      </template>

      <br>
      <v-expansion-panels>
        <v-expansion-panel>
          <v-expansion-panel-header>Raw Data</v-expansion-panel-header>
          <v-expansion-panel-content>
            <pre>{{ paper }}</pre>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-container>
  </div>
</template>

<script>
import Vue from 'vue'

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
      var componentName = `${this.paper.kind || "data"}-paper`;
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
    },

    fetchData () {
      this.alert = {};
      this.paper = null;
      this.loading = true;

      var tokens = [
        this.catalogName,
        this.paperName,
        this.paperAction,
        ...this.paperKeys
      ]
      var path = `/!/${tokens.join('/')}`;
      console.log({ path });
      
      fetch(path)
        .then(response => response.json())
        .then(data => this.openPaper(data))
        .catch(error => this.alert = {
          type: 'error',
          message: 'O servidor não respondeu como esperado.',
          detail: `Certifique-se de que o servidor esteja operando normalmente e
              disponível nesta mesma rede.`,
          fault: error
        })
        .finally(() => {
          this.loading = false;
        });
    }
  }
}
</script>
