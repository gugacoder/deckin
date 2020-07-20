<template lang="pug"> 
  v-container(
    :class="style"
  )
    the-paper-header(
      app
      v-bind="parameters"
    )

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

<style scoped>
.xs-size {
  max-width: 300px;
}
.xs-size .v-card__title {
  display: block;
}
</style>

<script>
import Vue from 'vue'
import lodash from 'lodash'
import PaperBase from './-PaperBase.vue'
import ThePaperHeader from './parts/ThePaperHeader.vue'
import { unknownPaper } from '@/helpers/PaperHelper.js'
import '@/helpers/StringHelper.js'

export default {
  extends: PaperBase,

  name: 'action-paper',

  components: {
    'the-paper-header': ThePaperHeader,
  },

  data: () => ({
    valid: true,
    message: null
  }),

  computed: {
    fields () {
      return this.paper.fields
    },

    style () {
      return this.paper.view.size
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

    async submit () {
      let ok

      this.message = null

      ok = this.$refs.form.validate()
      if (!ok) return

      let payload = {
        form: { ...this.paper.data },
        data: []
      }
      
      let link = this.paper.getLink('action')
          || this.paper.getLink('self')
          || { href: this.$browser.href(this) }

      let { href, data } = link
      lodash.merge(payload.form, data)

      let paper = await this.$browser.request(href, payload) || unknownPaper

      let paperLink = paper.getLink('self')
          || this.paper.getLink('self')
          || { href: this.$browser.href(this) }
      let path = this.$browser.routeFor(paperLink.href)
      if (path !== this.$route.path) {
        this.$router.push(path)
      }

      if (paper.kind === 'validation') {
        // TODO: Falta exibir a validação na interface...
      }

      if (paper.kind === 'paper') {
        this.setPaper(paper)
      }
    },

    cancel () {
      this.$refs.form.reset()
      this.$router.back()
      this.$router.go()
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
  }
}
</script>