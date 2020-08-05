<template lang="pug">
  v-container.action-slice(
    class="fill-height"
    fluid
  )
    v-row(  
      align="center"
      justify="center"
    )
      v-col(
        cols="12"
        sm="8"
        md="4"
        align="center"
        justify="center"
      )
        v-form(
          ref="form"
          v-model="valid"
          lazy-validation
          :class="extent || paperExtent"
          @submit.prevent="submit"
        )
          div
            p {{ message }}

          v-card(
            flat
            :class="extent || paperExtent"
          )
            v-card-text
              component.stretch(
                v-for="field in fields"
                :key="field.view.name"
                v-bind="createWidget(field)"
              )

            v-card-actions
              v-spacer

              slot(
                name="actionBar"
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
</template>

<style scoped lang="scss">
.flex-container {
  display: inline-flex;
  flex-wrap: wrap;
  width: 100%;
}

.stretch {
  width: 100%;
}

.button-row {
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
    actionName: {
      type: String,
      // Quando não informado o próprio Paper é considerado a ação
      required: false,
    },
    extent: {
      type: String,
      required: false
    }
  },

  data: () => ({
    valid: true,
    message: null
  }),

  computed: {
    action () {
      if (this.actionName === null) {
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
      console.log(this.action.view)
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
        return
      }

      this.$emit('paperReceived', paper)
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