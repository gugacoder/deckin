<template lang="pug">
  v-container.action-slice.pa-0(
    fluid
  )
    v-form(
      ref="form"
      v-model="valid"
      lazy-validation
      @submit.prevent="submit"
    )
      v-card.mx-auto.x-bg-translucent(
        :flat="flat"
        :class="flat ? 'pa-0' : 'pa-3'"
      )
        v-card-title(
          v-if="!noTitle"
        )
          | {{ title }}

        v-card-text.pa-0.pl-4.pt-2
          div.x-flex
            component(
              v-for="field in fields"
              :key="field.view.name"
              :ref="field.view.name"
              v-bind="createWidget(field)"
            )

        v-card-actions
          slot(
            name="action"
          )
            v-spacer(
              v-if="right || center"
            )

            v-btn.mr-2(
              type="submit"
              color="primary"
            )
              | {{ linkTitle }}

            v-btn.mr-2(
              v-if="$listeners.cancel"
              @click="$emit('cancel')"
            )
              | Cancelar

            v-spacer(
              v-if="!right || center"
            )
</template>

<style scoped lang="scss">
.x-flex {
  display: flex;
  flex-wrap: wrap;
}

.x-bg-translucent {
  background-color: rgba(0,0,0,0);
}

.x-stretch {
  width: 100%;
}

.xs-extent {
  width: 155px;
}

.sm-extent {
  width: 300px;
}

.md-extent {
  width: 445px;
}

.lg-extent {
  width: 590px;
}

.xl-extent {
  width: 745px;
}

.xs-extent .v-card__title,
.sm-extent .v-card__title,
.md-extent .v-card__title,
.lg-extent .v-card__title,
.xl-extent .v-card__title {
  display: block;
}
</style>

<script>
import Vue from 'vue'
import lodash from 'lodash'
import SliceBase from './-SliceBase.vue'
import { unknownPaper } from '@/helpers/PaperHelper.js'
import '@/helpers/StringHelper.js'

export default {
  extends: SliceBase,

  name: 'action-slice',
  
  props: {
    // Quando não informado o próprio Paper é considerado a ação
    actionName: {
      type: String,
    },

    flat: {
      type: Boolean,
    },

    extent: {
      type: String,
    },

    noTitle: {
      type: Boolean
    },

    right: {
      type: Boolean,
    },

    center: {
      type: Boolean,
    },
  },

  data: () => ({
    valid: true,
  }),

  computed: {
    action () {
      if (!this.actionName) {
        return this.paper
      } else {
        let actions = this.paper.actions
        let action = actions.filter(action => action.view.name === this.actionName)[0]
        return action
      }
    },

    title () {
      return this.action.view.title
    },

    link () {
      let links = this.action.links
      let link = links.filter(link => link.rel === 'action')[0]
      if (!link) {
        link = links.filter(link => link.rel === 'self')[0]
      }
      return link || {}
    },

    linkTitle () {
      return this.link.title || `Executar ${this.title}`
    },

    fields () {
      return this.action.fields.filter(field => !field.view.hidden)
    },

    paperExtent () {
      return this.action.view.extent
    }
  },

  methods: {
    createWidget (field) {
      let name = `${field.kind.toHyphenCase()}-widget`
      if (!Vue.options.components[name]) {
        name = 'invalid-widget'
      }
      return {
        is: name,
        paper: this.action,
        fieldName: field.view.name
      }
    },

    submit () {
      if (this.$listeners.submit) {
        this.$emit('submit')
      } else {
        this.fetchData()
      }
    },

    async fetchData () {
      let ok

      this.clearValidation()

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

      if (paper.kind === 'validation') {
        return this.showValidation(paper)
      }

      let paperLink = paper.getLink('self')
          || this.paper.getLink('self')
          || { href: this.$browser.href(this) }
      let path = this.$browser.routeFor(paperLink.href)
      if (path !== this.$route.path) {
        this.$router.push(path)
      }
      
      this.$emit('paperReceived', paper)
    },
    
    clearValidation () {
      if (this.$refs) {
        Object.keys(this.$refs).forEach(key => {
          let ref = this.$refs[key]
          let widgets = Array.isArray(ref) ? ref : [ ref ]
          widgets.forEach(widget => {
            if (widget.alert) {
              this.$set(widget, 'alert', {})
            }
          })
        })
      }
      this.$emit('alert', {
        type: null,
        message: null,
        detail: null,
      })
    },

    // paper must be of kind 'validation'
    showValidation (paper) {
      let issues = []
      let focusedWidget
      
      if (paper.data.message) {
        issues.push(paper.data)
      }

      if (paper.data.issues) {
        issues.push(...paper.data.issues)
      }
      
      issues.forEach(issue => {
        let alert = {
          type: (issue.severity || 'warning').toLowerCase(),
          message: issue.message || 'Falhou a tentativa de enviar os dados para o servidor.',
          detail: issue.detail
        }

        let widget = issue.field ? this.$refs[issue.field] : null
        if (Array.isArray(widget)) {
          widget = widget[0]
        }

        if (widget) {
          focusedWidget = focusedWidget || widget
          this.$set(widget, 'alert', alert)
        } else {
          this.$emit('alert', alert)
        }
      })

      focusedWidget && focusedWidget.focus()
    },

    // showValidation (message, fieldName) {
    //   let field = this.fields.filter(x => x.name === fieldName).shift()
    //   if (field) {
    //     field.fault = message
    //     field.rules = [ ( ) => !field.fault || field.fault ]
    //   } else {
    //     field = this.fields[0]
    //     this.message = message
    //   }

    //   let widget = this.$refs.widgets[0]
    //   widget && widget.focus()
    // },
  }
}
</script>