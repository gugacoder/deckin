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
import { createNamespacedHelpers } from 'vuex'
import lodash from 'lodash'
import { API_PREFIX } from '@/plugins/BrowserPlugin.js'
import '@/helpers/StringHelper.js'
import { createPaperPromise, unknownPaper } from '@/helpers/PaperHelper.js'

const { mapActions, mapGetters } = createNamespacedHelpers('paper')

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
    ...mapGetters([
      'getPaper'
    ]),

    title () {
      return this.paper.view.title
    },
    
    fields () {
      return this.paper.fields
    }
  },

  methods: {
    ...mapActions([
      'storePaper',
      'purgePaper',
      'fetchPaper',
      'ensurePaper',
    ]),

    nameWidget (field) {
      let pascalName = field.kind.toHyphenCase()
      let componentName = `${pascalName}-widget`
      if (!Vue.options.components[componentName]) {
        componentName = 'invalid-widget'
      }
      return componentName;
    },

    makeLink (rel) {
      let x = this.paper.links.filter(x => x.rel === rel)[0]
      if (!x) return ''

      let href = x.href
      if (!href) return ''
 
      let path = href.split('!/')[1]
      if (!path) return ''
      
      return `/Papers/${path}`
    },

    async submit () {
      let ok

      this.message = null

      ok = this.$refs.form.validate()
      if (!ok) return

      let payload = {
        form: { ...this.paper.data },
        data: []
      }
      
      let link = this.paper.links.filter(link => link.rel == "action")[0]
      let linkPayload = lodash.merge({}, link.data || {}, payload)

      await this.fetchPaper({ href: link.href, payload: linkPayload })
      let paper = this.getPaper(link.href) || unknownPaper

      // Validando possível redirecionamento...
      //
      ok = this.handleForward(paper)
      if (ok) return
      
      // O que fazer?...
    },

    async handleForward (paper) {
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
        return true
      }
      
      return false
    },

    cancel () {
      this.$refs.form.reset()
      this.$router.push('/')
    },

    showValidation (message, fieldName) {
      let field = this.fields.filter(x => x.name === fieldName).shift()
      if (field) {
        field.fault = message
        field.rules = [ ( ) => !field.fault || field.fault ]
      } else {
        field = this.fields[0]
        this.message = message
      }

      let widget = this.$refs.widgets[0]
      widget && widget.focus()
    },
/*    
    handleResponse (paper) {
      handlePaper(paper, this.showValidation, this.openPaper, this.redirectPaper)
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
    */
  }
}
</script>