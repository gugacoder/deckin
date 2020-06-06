<template>
  <div class="action-paper">
    <v-card
      class="mx-auto"
    >
      <v-card-title>
        {{ title }}
      </v-card-title>

      <v-card-text>
        <v-form
          @submit.prevent="submit()"
        >
          <div
            v-for="field in fields"
            :key="field.name"
          >
            <div>
              <v-text-field
                v-model="field.value"
                :type="field.kind"
                :label="field.title || field.name"
              >
              </v-text-field>
            </div>
          </div>

          <v-btn
            type="submit"
            color="primary"
            class="mr-2"
          >
            Confirmar
          </v-btn>

          <v-btn
            class="mr-2"
            @click="cancel()"
          >
            Cancelar
          </v-btn>
        </v-form>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import Vue from 'vue'
import lodash from 'lodash'

export default {
  name: 'action-paper',

  props: [
    'catalogName',
    'paperName',
    'paperAction',
    'paperKeys',
    'paper'
  ],

  data: () => ({
  }),

  computed: {
    title () {
      if (this.paper.title) return this.paper.title
      if (this.paper.view && this.paper.view.title) return this.paper.view.title

      var tokens = [
        this.paperName, 
        this.paperAction,
        ...this.paperKeys
      ]

      return tokens.join(' / ')
    },

    fields () {
      lodash.startCase('');
      if (!this.paper) return []

      if (this.paper.fields) {
        var host = this.paper;
        return Object.keys(host.fields).map(name => ({
          name,
          ...host.fields[name],
          ...(host.fields[name].view ? host.fields[name].view : {}),
          get value() {
            return host.data ? host.data[name] : null
          },
          set value(value) {
            if (!host.data) {
              Vue.set(host, 'data', {})
            }
            Vue.set(host.data, name, value)
          }
        }))
      }

      return []
    }
  },

  methods: {
    makeLink (rel) {
      var link = this.paper.links.filter(link => link.rel === rel)[0]
      if (!link) return ''

      var href = link.href
      if (!href) return ''
 
      var path = href.split('!/')[1]
      if (!path) return ''

      return `/Papers/${path}`
    },

    submit () {
      alert("It works!")
    },

    cancel () {
      this.$router.push('/')
    }
  }
}
</script>