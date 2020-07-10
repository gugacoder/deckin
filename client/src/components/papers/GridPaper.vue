<template lang="pug"> 
  div.grid-paper
    the-paper-header(
      v-bind="parameters"
    )
      v-btn(
        icon
        :color="autoRefresh ? 'white' : 'accent'"
        @click="autoRefresh = !autoRefresh"
      )
        v-icon {{ autoRefresh ? 'mdi-refresh-circle' : 'mdi-refresh' }}

    v-data-table(
      :headers="cols"
      :items="rows"
      :disable-pagination="true"
      :disable-sort="true"
      :hide-default-footer="true"
      item-key="uid"
      dense
    )
      template(
        v-slot:item="{ item, headers }"
      )
        tr
          td.text-start(
            v-for="header in headers"
            :key="`${item.key}-${header.value}`"
            nowrap
          )
            | {{ item[header.value] }}
</template>

<script>
import Vue from 'vue'
import BasePaperPart from './BasePaperPart'
import lodash from 'lodash'
import moment from 'moment'
import '@/helpers/StringHelper'
import { unknownPaper } from '@/helpers/PaperHelper'
import ThePaperHeader from './parts/ThePaperHeader'

export default {
  extends: BasePaperPart,

  name: 'grid-paper',

  components: {
    'the-paper-header': ThePaperHeader,
  },

  data: () => ({
    timer: null,
    autoRefresh: false,
  }),

  created () {
    this.onRouteCreated();
  },

  watch: {
    '$route': 'onRouteChanged',
    'content.paper': 'onPaperChanged',
  },

  computed: {
    href () {
      return this.$browser.href(this)
    },

    autoRefreshSeconds () {
      let seconds = this.paper.view.autoRefresh
      if (!seconds)
        return null
      if (!Number.isInteger(seconds))
        seconds = 2
      return seconds
    },

    fields () {
      return this.paper.fields
    },

    cols () {
      let headers = this.fields.filter(x => !x.hidden).map(field => ({
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
            data[key] = moment(value).format('DD/MM/YY hh:mm:ss')
          }
        })
        if (!data.key) {
          data.key = this.paper.embedded.indexOf(x)
        }
        return data
      })
    }
  },

  methods: {
    onRouteCreated () {
      this.startAutoRefreshData()
      this.autoRefresh = this.paper.view.autoRefresh
    },

    onRouteChanged () {
      this.autoRefresh = this.paper.view.autoRefresh
    },

    onPaperChanged () {
    },

    startAutoRefreshData() {
      if (!this.timer) {
        this.timer = () => {
          let milliseconds = (this.autoRefreshSeconds || 2) * 1000
          setTimeout(() => {
            if (this.autoRefresh) {
              this.refreshData().then(() => this.timer())
            } else {
              this.timer()
            }
          }, milliseconds)
        }
        this.timer()
      }
    },

    async refreshData() {
      let href = this.href
      let data = this.$route.query

      let paper = await this.$browser.request(href, data) || unknownPaper

      let self = paper.getLink('self') || { href: this.href }
      let path = this.$browser.routeFor(self.href)
      if (path === this.$route.path) {
        this.content.paper = paper
      }
    },

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