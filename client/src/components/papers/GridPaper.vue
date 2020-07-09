<template lang="pug"> 
  div.grid-paper
    v-card(
      flat
      class="mx-auto"
    )
      v-card-title
        | {{ title }}

      v-card-text

        v-data-table(
          :headers="cols"
          :items="rows"
          :disable-pagination="true"
          :disable-sort="true"
          :hide-default-footer="true"
          item-key="uid"
          dense
        )
</template>

<script>
import Vue from 'vue'
import BasePaper from './BasePaper'
import lodash from 'lodash'
import moment from 'moment'
import '@/helpers/StringHelper.js'
import { unknownPaper } from '@/helpers/PaperHelper.js'

export default {
  extends: BasePaper,

  name: 'grid-paper',

  data: () => ({
  }),

  computed: {
    fields () {
      return this.paper.fields
    },

    cols () {
      console.log(this.fields)
      let headers = this.fields.map(field => ({
        value: field.data.name,
        text: field.view.title || field.data.name,
        sortable: false
      }))
      return headers
    },

    rows () {
      return this.paper.embedded.filter(x => x.kind === 'data').map(x => {
        let data = Object.assign({}, x.data)
        Object.keys(data).forEach(key => {
          let value = data[key]
          if (value instanceof Date) {
            data[key] = moment(value).format('DD/MM/YY HH:MM:SS')
          }
        })
        if (!data.uid) {
          data.uid = data[0]
        }
        return data
      })
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