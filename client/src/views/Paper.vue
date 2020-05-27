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
        - Title
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
        <h1>It works!</h1>
      </template>
    </v-container>
  </div>
</template>

<script>
export default {
  // Strategy: Fetching After Navigation

  name: 'Paper',

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

  created () {
    this.fetchData();
  },

  watch: {
    '$route': 'fetchData'
  },

  methods: {
    fetchData () {
      this.alert = this.paper = null;
      this.loading = true;

      fetch('/!/Keep.Paper/Home/Index')
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => this.alert = {
          type: 'error',
          message: 'O servidor não respondeu como esperado.',
          detail: `Certifique-se de que o servidor esteja operando normalmente e
              disponível nesta mesma rede.`,
          fault: error
        })
        .finally(() => this.loading = false);
    }
  }
}
</script>
