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
              :is="nameWidget(field)"
              :field="field"
              ref="widgets"
            )

          div
            p {{ message }}

          div
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
import '@/helpers/StringOperations.js'
import { fetchPaper, handlePaper } from '@/services/PaperService.js'

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
    message: null
  }),

  computed: {
    title () {
      return this.paper.view.title
    },
    
    fields () {
      return this.paper.fields
    }
  },

  methods: {
    nameWidget (field) {
      let pascalName = field.kind.toHyphenCase()
      let componentName = `${pascalName}-widget`
      if (!Vue.options.components[componentName]) {
        componentName = 'invalid-widget'
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
      this.message = null

      var ok = this.$refs.form.validate()
      if (!ok) return

      var payload = {
        form: { ...this.paper.data },
        data: []
      }
      
      var target = this.paper.links.filter(x => x.rel == "action").shift()

      fetchPaper(target.href, payload)
        .then(data => this.handleResponse(data))
        .catch(error => this.handleFailure(error))
    },

    cancel () {
      this.$refs.form.reset()
      this.$router.push('/')
    },

    handleResponse (paper) {
      handlePaper(paper, this.showValidation, this.openPaper, this.redirectPaper)
    },

    showValidation (message, fieldName) {
      var field = this.fields.filter(x => x.name === fieldName).shift()
      if (field) {
        field.fault = message
        field.rules = [ ( ) => !field.fault || field.fault ]
      } else {
        field = this.fields[0]
        this.message = message
      }

      var widget = this.$refs.widgets[0]
      widget && widget.focus()
    },

    openPaper (paper) {
      console.log({ paper })
    },

    redirectPaper (href) {
      this.$router.push(href);
    },

    handleFailure (error) {
      console.log(error)
    }
  }
}
</script>