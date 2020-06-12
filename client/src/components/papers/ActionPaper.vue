<template lang="pug"> 
  div(
    class="action-paper"
  )
    v-card(
      class="mx-auto"
    )
      v-card-title
        | {{ title }}

      v-card-text
        v-form(
          ref="form"
          v-model="valid"
          lazy-validation
          @submit.prevent="submit()"
        )
          div(
            v-for="field in fields"
            :key="field.name"
          )
            //- Instância do Widget
            component(
              :is="identifyWidget(field)"
              :field="field"
            )

          v-btn(
            type="submit"
            color="primary"
            class="mr-2"
          )
            | Confirmar

          v-btn(
            class="mr-2"
            @click="cancel()"
          )
            | Cancelar
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
    valid: true,
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
      if (!this.paper && !this.paper.fields) return []

      var host = this.paper
      var field = Object.keys(host.fields).map(function(name) {
        var view = ({
          ...host.fields[name],
          ...(host.fields[name].view ? host.fields[name].view : {}),
        })
        return {
          ...view,
          name,
          label: view.title || view.name,
          fault: null,
          
          get value() {
            return host.payload ? host.payload[name] : null
          },
          set value(value) {
            if (!host.payload) {
              Vue.set(host, 'payload', {})
            }
            Vue.set(host.payload, name, value)
          },
        }
      })

      return Object.assign({}, field)
    }
  },

  methods: {
    identifyWidget (field) {
      var pascalName = lodash.startCase(field.kind).replace(' ', '')
      var componentName = `${pascalName}Widget`
      if (!Vue.options.components[componentName]) {
        componentName = 'InvalidWidget'
      }
      return componentName;
    },

    makeLink (rel) {
      var x = this.paper.links.filter(x => x.rel === rel)[0]
      if (!x) return ''

      var href = x.href
      if (!href) return ''
 
      var path = href.split('!/')[1]
      if (!path) return ''
      
      return `/Papers/${path}`
    },

    async submit () {
      /*
      var ok = this.$refs.form.validate()
      if (!ok) return

      var payload = {
        form: {},
        data: []
      }
      
      this.fields.forEach(x => payload.form[x.name] = x.value)

      if (this.paper.data) {
        payload.data.push(this.paper.data)
      }

      var target = this.paper.links.filter(x => x.rel == "action").shift()
      var uri = target.href

      fetch(uri, {
          method: 'post',
          body: JSON.stringify(payload)
        })
        .then(response => response.json())
        .then(data => this.postSubmit(data, null))
        .catch(error => this.postSubmit(null, error))
      */

      var w = this.fields[0];
      console.log(w.label)
      this.$set(w, 'label', 'Tananana')
      console.log(w.label)

    },

    cancel () {
      this.$refs.form.reset()
      this.$router.push('/')
    },

    //postSubmit (response, error) {
    postSubmit (response) {
      //console.log(response, error)
      if (response.kind === 'validation') {
        var field = this.fields.filter(x => x.name === response.data.field).shift()
        if (field) {
          field.fault = response.data.message
          field.rules = [ ( ) => !field.fault || field.fault ]
        }
        field.label = "Tananana"
        console.log(field)
      }
    }
  }
}
</script>