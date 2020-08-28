<template lang="pug">
  //- 
  //- HELP: Métodos de controle de exibição:
  //-   .d-none.d-sm-flex => Não exibido em celulares 
  //-   .d-flex.d-sm-none => Exibido somente em celulares
  //-   $vuetify.breakpoint.xsOnly => Diz se é celular
  //-
  v-app.grid-paper
    the-header(
      :catalog="catalogName"
      :caption="title"
      noTitle
    )
      template(
        slot="first"
      )
        the-app-menu-button(
          v-model="menu"
        )

      template(
        slot="left"
      )
        template(
          v-if="filter.action"
        )
          v-btn.x-toolbar-btn(
            :icon="$isMobile"
            :text="!$isMobile"
            :outlined="!$isMobile"
            :class="{ 'x-mobile': $isMobile }"
            @click="filter.visible = !filter.visible"
            v-shortkey="['esc']"
            @shortkey="filter.visible = !filter.visible"
          )
            v-icon mdi-filter

            span(
              v-if="!$isMobile"
            )
              | Filtro

        v-btn.x-toolbar-btn.x-dropdown-btn(
          v-if="!$isMobile"
          text
          outlined
          @click="refreshData(true)"
          v-shortkey="['enter']"
          @shortkey="refreshData(true)"
        )
          v-icon.x-flip(
            v-if="autoRefresh.enabled"
          )
            | mdi-history

          v-icon(
            v-if="!autoRefresh.enabled"
          )
            | mdi-refresh

          span(
            v-if="!$isMobile"
          )
            span Atualizar
              span.x-lowercase(
                v-if="autoRefresh.enabled"
              )
                | : {{ autoRefreshTip }}

          v-menu(
            bottom
            offset-y
            close-on-click
            close-on-content-click
          )
            template(
              v-slot:activator="{ on, attrs }"
            )
              v-btn(
                v-bind="attrs"
                v-on="on"
                text
                small
              )
                v-icon.pa-4 mdi-chevron-down

            v-list
              v-list-item(
                v-for="interval in autoRefresh.intervals"
                :key="`interface-${interval.seconds}`"
                link
                dense
                @click="autoRefresh.seconds = interval.seconds"
              )
                v-list-item-title
                  span(
                    :class="{ 'primary--text': (autoRefresh.seconds === interval.seconds) }"
                  )
                    template(
                      v-if="interval.seconds === 0"
                    )
                      span.font-weight-light Atualizar &nbsp;
                      span.font-weight-medium manualmente

                    template(
                      v-else
                    )
                      span.font-weight-light Atualizar a cada &nbsp;
                      span.font-weight-medium {{ interval.title }} &nbsp;

                v-list-item-action
                  v-icon(
                    color="primary"
                  )
                    | {{ (autoRefresh.seconds === interval.seconds) ? 'mdi-check' : undefined }}

        v-btn.x-toolbar-btn(
          v-if="$isMobile"
          icon
          :class="{ 'x-mobile': $isMobile }"
          @click="refreshData(true)"
        )
          v-icon mdi-refresh

        v-btn.x-toolbar-btn(
          v-if="$isMobile"
          icon
          :class="{ 'x-mobile': $isMobile, 'x-btn-pressed': autoRefresh.enabled }"
          @click="setAutoRefresh(!autoRefresh.enabled)"
        )
          v-icon.x-flip mdi-history

      template(
        slot="right"
      )
        the-app-user-menu-button(
          v-model="userMenu"
        )

    the-app-menu(
      v-model="menu"
      :catalog="catalogName"
      :caption="title"
    )
    
    the-app-user-menu(
      v-model="userMenu"
    )

    the-footer(
      ref="theFooter"
      :busy="busy"
      @refresh="() => refreshData(true)"
    )
      template(
        slot="left"
      )
        v-menu(
          :close-on-click="true"
          :close-on-content-click="true"
          :offset-x="false"
          :offset-y="true"
          top
          dense
        )
          template(
            v-slot:activator="{ on, attrs }"
          )
            v-btn(
              v-bind="attrs"
              v-on="on"
              text
              rounded
            )
              span
                big {{ rows.length }} &nbsp;
                small.font-weight-light {{ rows.length === 1 ? 'registro' : 'registros' }}

          v-list
            v-list-item(
              v-for="size in pagination.pageSizes"
              :key="`pageSize-${size}`"
              link
              dense
              @click="pagination.pageSize = size"
            )
              v-list-item-title
                span(
                  :class="{ 'primary--text': (pagination.pageSize === size) }"
                )
                  template(
                    v-if="size === 0"
                  )
                    span.font-weight-light Exibir &nbsp;
                    span.font-weight-medium todos &nbsp;
                    span.font-weight-light os registros

                  template(
                    v-else
                  )
                    span.font-weight-light Exibir até &nbsp;
                    span.font-weight-medium {{ size }} &nbsp;
                    span.font-weight-light registros

              v-list-item-action
                v-icon(
                  color="primary"
                )
                  | {{ (pagination.pageSize === size) ? 'mdi-check' : undefined }}

    app-drawer(
      v-if="filter.action"
      v-model="filter.visible"
      title="Filtro"
      :noFooter="autoRefresh.enabled"
    )
      template(
        slot="footer"
      )
        v-btn(
          color="primary"
          @click="refreshData(true)"
        )
          | Aplicar

      action-slice(
        ref="filterSlice"
        v-bind="filterSlice"
        flat
        noTitle
        extent="sm-extent"
        @submit="refreshData(true)"
      )
        template(
          v-slot:action
        )
          div

    the-content(
      fluid
      noMargins
    )
      the-alert(
        v-model="alert"
      )

      //-
      //- GRADE DE DADOS
      //-
      v-data-table.ma-0(
        :headers="cols"
        :items="rows"
        :fixed-header="true"
        :disable-filtering="true"
        :disable-pagination="true"
        :disable-sort="true"
        :hide-default-header="true"
        :hide-default-footer="true"
        item-key="uid"
        dense
      )
        template(
          v-slot:header="{ props }"
        )
          thead
            tr
              th(
                style="z-index: 0"
                v-for="header in props.headers"
              )
                | {{ header.text }}
      
        template(
          v-slot:item="{ item, headers, index, isMobile }"
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
</template>

<style scoped>
.x-lowercase {
  text-transform: lowercase;
}

.x-toolbar-btn {
  margin-left: 4px !important;
  padding-left: 12px !important;
  padding-right: 12px !important;
}

.x-toolbar-btn:not(.v-btn--round) .v-icon {
  margin-right: 4px !important;
}

.x-dropdown-btn {
  padding-right: 4px !important;
}
           
.x-dropdown-btn .v-btn{
  margin-left: 8px !important;
  padding: 0 !important;
  width: 30px;
  min-width: 30px;
}

.x-flip {
  transform: scaleX(-1);
}

.x-btn-pressed {
  background-color: rgba(255,255,255,.2) !important;
}

.x-style-trace {
  color: var(--v-secondary-lighten2);
}

/*
.x-style-default {
  color: var(--v-secondary-base);
}
*/

.x-style-information {
  color: var(--v-info-base);
}

.x-style-success {
  color: var(--v-success-base);
}

.x-style-warning {
  color: var(--v-warning-base);
}

.x-style-danger {
  color: var(--v-error-base);
}
</style>

<script>
import Vue from 'vue'
import moment from 'moment'
import lodash from 'lodash'

import PaperBase from './-PaperBase.vue'
import TheHeader from '@/components/layout/TheHeader.vue'
import TheContent from '@/components/layout/TheContent.vue'
import TheFooter from '@/components/layout/TheFooter.vue'
import TheAppMenu from '@/components/layout/TheAppMenu.vue'
import TheAppMenuButton from '@/components/layout/TheAppMenuButton.vue'
import TheAppUserMenu from '@/components/layout/TheAppUserMenu.vue'
import TheAppUserMenuButton from '@/components/layout/TheAppUserMenuButton.vue'
import TheAlert from '@/components/layout/TheAlert.vue'
import AppTitle from '@/components/layout/AppTitle.vue'
import AppDrawer from '@/components/layout/AppDrawer.vue'

import '@/helpers/StringHelper.js'
import { unknownPaper } from '@/helpers/PaperHelper.js'

export default {
  extends: PaperBase,

  name: 'grid-paper',

  components: {
    TheHeader,
    TheContent,
    TheFooter,
    TheAppMenu,
    TheAppMenuButton,
    TheAppUserMenu,
    TheAppUserMenuButton,
    TheAlert,
    AppTitle,
    AppDrawer,
  },

  data: () => ({
    menu: false,
    userMenu: false,
    busy: false,
    alert: null,

    uid: Date.now(),
    lastRequestId: 0,

    filter: {
      action: null,
      visible: false,
      expanded: false,
    },

    autoRefresh: {
      enabled: false,
      seconds: 0,
      lastRun: 0,
      intervals: [
        { seconds: 0    , tip: ''   , title: ''            },
        { seconds: 1    , tip: '1s' , title: '1 segundo'   },
        { seconds: 2    , tip: '2s' , title: '2 segundos'  },
        { seconds: 3    , tip: '3s' , title: '3 segundos'  },
        { seconds: 5    , tip: '5s' , title: '5 segundos'  },
        { seconds: 10   , tip: '10s', title: '10 segundos' },
        { seconds: 60   , tip: '1m' , title: '1 minuto'    },
      ]
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
    dark: {
      get () {
        return this.$vuetify.theme.dark
      },

      set (value) {
        this.$vuetify.theme.dark = value
      }
    },

    href () {
      return this.$browser.href(this)
    },

    fields () {
      return this.paper.fields
    },

    filterSlice () {
      return {
        paper: this.filter.action
        //paper: this.paper,
        //actionName: 'filter'
      }
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
          data.style = `x-style-${style ? this.stylize(style) : null}`
        }

        return data
      })
    },

    autoRefreshTip () {
      let seconds = this.autoRefresh.seconds
      let intervals = this.autoRefresh.intervals 
      let interval = intervals.filter(x => x.seconds === seconds)[0]
      return interval && interval.tip
    },
  },

  watch: {
    'content.paper': 'onPaperChanged',
    'autoRefresh.seconds': 'onAutoRefreshChanged',
  },

  created () {
    this.onPaperChanged()
    this.setTimers()
  },

  destroyed() {
    this.clearTimers()
  },

  methods: {
    onAutoRefreshChanged () {
      let enabled = (this.autoRefresh.seconds > 0)
      if (enabled !== this.autoRefresh.enabled) {
        this.autoRefresh.enabled = enabled
      }
    },

    onPaperChanged () {

      // Auto refresh
      //
      let seconds = this.paper.view.design.autoRefresh || 0
      if (!Number.isInteger(seconds) || seconds < 0) {
        seconds = 0
      }
      this.autoRefresh.enabled = seconds > 0
      this.autoRefresh.seconds = seconds

      console.log({seconds})
      
      // Paginacao
      //
      let page = this.paper.view.design.page
      this.pagination.enabled = !!page
      if (this.pagination.enabled)
      {
        let size = this.pagination.pageSizes.filter(size => page.limit === size)[0]
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
      this.autoRefresh.timer = setInterval(this.tickTimer, 100)
    },

    clearTimers() {
      clearInterval(this.autoRefresh.timer)
      this.autoRefresh.timer = undefined
    },

    tickTimer() {
      if (!this.autoRefresh.enabled || this.busy)
        return

      let timeout = this.autoRefresh.lastRun + (1000 * this.autoRefresh.seconds)
      if (timeout > Date.now())
        return
      
      this.autoRefresh.lastRun = Date.now()
      this.refreshData()
    },

    applyFilter () {
      this.$refs.filterSlice.submit()
    },

    async refreshData(force) {
      try {
        if (this.busy && !force)
          return
          
        this.busy = true
        
        let requestId = this.lastRequestId = ((this.lastRequestId + 1) % 100)
        let href = this.href

        let payload = {}

        if (this.filter.action) {
          payload.form = this.filter.action.data
        }

        if (this.pagination.enabled) {
          payload.pagination = {
            offset: 0,
            limit: this.pagination.pageSize
          }
        }
        
        let paper = await this.$browser.request(href, payload) || unknownPaper

        let self = paper.getLink('self') || { href: this.href }
        let path = this.$browser.routeFor(self.href)
        if (path === this.$route.path) {
          if (force || this.autoRefresh.enabled) {
            if (requestId === this.lastRequestId) {
              this.updateData(paper)
            }
          }
        }

        this.busy = false
      } catch {
        // XXX: o que fazer com essa falha?
        this.busy = false
      }
    },

    updateData (paper) {
      switch (paper.kind) {
        case 'paper': {
          let incomingData = paper.embedded.filter(e => e.kind === 'data')
          let embedded = this.paper.embedded.filter(e => e.kind !== 'data')
          embedded.push(...incomingData)
          
          this.$set(this.paper, 'embedded', embedded)
          this.setAlert(null)

          break
        }

        case 'fault': {
          let fault = paper.data
          let detail = Array.isArray(fault.reason)
            ? fault.reason.join('\n')
            : fault.reason
          
          this.setAlert({
            type: 'error',
            message: 'O servidor não está entregando dados para atualização da grade.',
            detail
          })
          break
        }
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

    /*
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
    */

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

    stylize (style) {
      if (Number.isInteger(style)) {
        var options = [
          'trace',        // Cinza
          'default',      // Preto
          'information',  // Azul
          'success',      // Verde
          'warning',      // Laranja
          'danger',       // Vermelho
        ]
        style = options[style] || 'default'
      }

      if (!lodash.isString(style)) {
        style = 'default'
      }

      return style.toHyphenCase()
    },
  }
}
</script>
