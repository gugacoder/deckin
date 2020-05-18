<template>
  <v-content>

<h1>{{ xxx }}</h1>

    <v-container>
      <v-card
        max-width="400"
        class="mx-auto"
        flat
      >
        <v-card-title>
          Formulário de acesso ao sistema
        </v-card-title>

        <v-card-text>
          Entre com a credencial de acesso criada para você pelo administrador do sistema.
        </v-card-text>

        <v-form>
          <v-card-text>
            <v-text-field
              v-model="username"
              :type="'text'"
              :rules="rules"
              name="username"
              label="Nome"
              hint="Nome de usuário ou email."
              outlined
              dense
            ></v-text-field>

            <v-text-field
              v-model="password"
              :type="'password'"
              :rules="rules"
              name="password"
              label="Senha"
              hint="Senha de usuário."
              outlined
              dense
            ></v-text-field>

            <v-btn
              color="primary"
            >
              Entrar
            </v-btn>
          </v-card-text>
        </v-form>
      </v-card>
    </v-container>
  </v-content>
</template>

<script>
import Vue from 'vue';
import { mapState, mapActions } from 'vuex'
import config from '@/config.js'

export default Vue.extend({
  name: 'Login',

  components: {
  },

  data: () => ({
    xxx: `${config.apiUrl}/Users/Authenticate`,
    username: '',
    password: '',
    rules: [
      value => !!value || 'Requerido.'
    ]
  }),

  computed: {
    ...mapState('account', ['status'])
  },

  created () {
    // garante que nenhum usuário esteja logado no início
    this.logout();
  },

  methods: {
    ...mapActions('account', ['login', 'logout']),

    handleSubmit () {
      const { username, password } = this;
      if (username && password) {
        this.login({ username, password })
      }
    }
  }
});
</script>
