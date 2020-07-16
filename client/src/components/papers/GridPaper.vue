<template lang="pug"> 
  div.grid-paper
    the-paper-header(
      v-bind="parameters"
      @click="refreshData(true)"
      :clipped="true"
    )
      //-
        //- Botão de ocultação do menu
        template(
          slot="left"
        )
          v-btn.v-btn.custom-menu-button(
            icon
            @click.stop="toggleMainMenuUp()"
          )
            v-icon(
              :color="menu.minified ? 'primary lighten-2' : null"
            )
              | mdi-menu

      //- Paginação
      v-menu(
        v-if="pagination.enabled"
        :close-on-click="true"
        :close-on-content-click="true"
        :offset-x="false"
        :offset-y="true"
      )
        //- Botão de páginação
        template(
          v-slot:activator="{ on, attrs }"
        )
          v-btn(
            v-bind="attrs"
            v-on="on"
            rounded
            depressed
            large
            color="primary"
          )
            span(
              v-if="pagination.pageSize > 0"
            )
              span.font-weight-thin
                small Exibindo até &nbsp;
                span.font-weight-medium
                  big {{ pagination.pageSize }} &nbsp;
                  span.font-weight-thin
                    small registros
            span(
              v-else
            )
              span.font-weight-thin
                small Exibindo &nbsp;
                span.font-weight-medium
                  big todos &nbsp;
                  span.font-weight-thin
                    small os registros

        //- Submenu de paginação
        v-list
          v-list-item(
            v-for="size in pagination.pageSizes"
            :key="size"
            @click="setPageSize(size)"
          )
            v-list-item-action
              v-icon(
                :color="pagination.pageSize === 0 ? 'primary' : ''"
              )
                | {{ pagination.pageSize === size ? 'mdi-radiobox-marked' : 'mdi-radiobox-blank' }}
            v-list-item-content
              span(
                v-if="size > 0"
              )
                span.font-weight-thin Exibir até &nbsp;
                  span.font-weight-medium {{ size }} &nbsp;
                    span.font-weight-thin registros
              span(
                v-else
              )
                span.font-weight-thin Exibir &nbsp;
                  span.font-weight-medium todos &nbsp;
                    span.font-weight-thin os registros      
      
      //- Autorefresh
      v-btn(
        rounded
        depressed
        large
        color="primary"
        @click="setAutoRefresh(!autoRefresh.timer)"
      )
        v-icon(
          :color="!!autoRefresh.timer ? '' : 'primary lighten-2'"
        )
          | mdi-history
        
        span(
          v-show="!!autoRefresh.timer"
        )
          span.font-weight-medium
            big {{ autoRefresh.seconds }}
            span.font-weight-thin
              small s

    //- Barra Lateral de Funções
    v-navigation-drawer(
      v-if="filter.action"
      :mini-variant="filter.minified"
      app
      clipped
      permanent
      :width="filter.expanded ? 590 : 300"
    )
      v-list.pa-0
        v-list-item.px-2.pt-1(
          dense
        )
          v-list-item-icon.mt-2px
            v-btn(
              icon
              @click.stop="filter.minified = !filter.minified"
            )
              v-icon mdi-filter

          v-list-item-title.subtitle-1 {{ filter.action.view.title || 'Filtro' }}

          v-btn(
            icon
            @click.stop="filter.expanded = !filter.expanded"
          )
            v-icon {{ filter.expanded ? 'mdi-chevron-left' : 'mdi-chevron-right' }}

      v-divider

      action-slice(
        v-show="!filter.minified"
        v-bind="filterProperties"
      )

    //- Tabela de dados
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
        tr(
          :class="item.style"
        )
          td.text-start(
            v-for="header in headers"
            :key="`${item.key}-${header.value}`"
            :nowrap="!(header.multiline || hasLineBreak(item[header.value]))"
          )
            | {{ item[header.value] }}
    
    the-paper-footer(
      v-bind="parameters"
    )
      v-btn(
        rounded
        depressed
        small
        color="white"
        @click="refreshData(true)"
      )
        span {{ rows.length }} &nbsp;
          span.font-weight-light {{ rows.length === 1 ? 'registro' : 'registros' }}

      v-btn(
        icon
        small
        @click="refreshData(true)"
      )
        v-progress-circular(
          :indeterminate="busy"
          rotate
          value="0"
          size="16"
          width="2"
          color="primary"
        )
</template>

<style scoped>
.mt-2px {
  margin-top: 2px;
}

.custom-menu-button {
  margin-left: -14px;
}

.custom-style-trace {
  color: gray;
}

.custom-style-default {
  color: var(--v-secondary-base);
}

.custom-style-information {
  color: var(--v-info-base);
}

.custom-style-success {
  color: var(--v-success-base);
}

.custom-style-warning {
  color: var(--v-warning-base);
}

.custom-style-danger {
  color: var(--v-error-base);
}
</style>

<script>
import Vue from 'vue'
import BasePaperPart from './BasePaperPart'
import lodash from 'lodash'
import moment from 'moment'
import '@/helpers/StringHelper'
import { unknownPaper } from '@/helpers/PaperHelper'
import ThePaperHeader from './parts/ThePaperHeader'
import ThePaperFooter from './parts/ThePaperFooter'

export default {
  extends: BasePaperPart,

  name: 'grid-paper',

  components: {
    'the-paper-header': ThePaperHeader,
    'the-paper-footer': ThePaperFooter,
  },

  data: () => ({
    uid: Date.now(),
    lastRequestId: 0,
    busy: false,

    filter: {
      action: null,
      minified: false,
      expanded: false,
    },

    autoRefresh: {
      enabled: false,
      seconds: 0,
      timer: null,
    },

    pagination: {
      enabled: false,
      pageSize: 0,
      pageSizes: [
        0,
        10,
        20,
        50,
        100,
        200,
        500,
        1000,
        2000,
        5000,
      ]
    },
  }),

  computed: {
    href () {
      return this.$browser.href(this)
    },

    fields () {
      return this.paper.fields
    },

    filterProperties () {
      return Object.assign({}, this.parameters, {
        action: this.filter.action
      })
    },

    cols () {
      let headers = this.fields.filter(x => !x.view.hidden).map(field => ({
        value: field.data.name,
        text: field.view.title || field.data.name,
        sortable: false,
        multiline: field.view.multiline
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
        
        let styleField = this.paper.fields.filter(x => x.view.useForStyle)[0]
        if (styleField) {
          let style = data[styleField.data.name]
          data.style = `custom-style-${style ? style.toHyphenCase() : null}`
        }

        return data
      })
    }
  },

  watch: {
    'content.paper': 'onPaperChanged',
  },

  created () {
    this.onPaperChanged()
    this.setTimers()
  },

  destroyed() {
    this.clearTimers()
  },

  methods: {
    onPaperChanged() {

      // Auto refresh
      //
      this.autoRefresh.enabled = !!this.paper.view.autoRefresh
      this.autoRefresh.seconds = this.paper.view.autoRefresh
      if (!Number.isInteger(this.autoRefresh.seconds)
          || this.autoRefresh.seconds <= 0) {
        this.autoRefresh.seconds = 2
      }
      
      // Paginacao
      //
      let page = this.paper.view.page
      this.pagination.enabled = !!page
      if (this.pagination.enabled)
      {
        let size = this.pagination.pageSizes.filter(size => page.limit == size)[0]
        this.pagination.pageSize = size || 0
      }

      // Filtragem
      this.filter.action = this.paper.actions.filter(a => a.view.name === 'filter')[0]
    },

    setAutoRefresh(enabled) {
      this.clearTimers()
      this.autoRefresh.enabled = enabled
      if (enabled) {
        this.setTimers()
      }
    },

    setPageSize(size) {
      this.pagination.pageSize = size
      this.refreshData(true)
    },

    setTimers() {
      this.clearTimers()
      if (this.autoRefresh.enabled) {
        let milliseconds = this.autoRefresh.seconds * 1000
        this.autoRefresh.timer = setInterval(this.refreshData, milliseconds)
      }
    },

    clearTimers() {
      if (this.autoRefresh.timer) {
        clearInterval(this.autoRefresh.timer)
        this.autoRefresh.timer = null
      }
    },

    async refreshData(force) {
      try {
        if (this.busy && !force)
          return

        this.busy = true
        
        let requestId = this.lastRequestId = ((this.lastRequestId + 1) % 100)
        let href = this.href

        let data = lodash.merge({}, this.$route.query)

        if (this.pagination.enabled) {
          let opts = {
            pagination: {
              offset: 0,
              limit: this.pagination.pageSize
            }
          }
          data = lodash.merge(data, opts)
        }

        let paper = await this.$browser.request(href, data) || unknownPaper

        let self = paper.getLink('self') || { href: this.href }
        let path = this.$browser.routeFor(self.href)
        if (path === this.$route.path) {
          if (force || this.autoRefresh.enabled) {
            if (requestId === this.lastRequestId) {
              this.content.paper = paper
            }
          }
        }

        this.busy = false
      } catch {
        // XXX: o que fazer com essa falha?
        this.busy = false
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

    hasLineBreak (text) {
      return /[\n\r]/.exec(text)
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