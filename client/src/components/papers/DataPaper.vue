<template lang="pug">
  div(
    class="data-paper"
  )
    v-card(
      flat
      class="mx-auto"
    )
      v-card-title
        | {{ title }} | {{ actionName }}

      v-card-text
        div(
          v-for="field in fields"
          :key="field.name"
        )
          p
            span(
              v-show="field.kind !== 'information'"
            )
              | {{ field.name }}
              
              br

            span(
              v-if="field.linkTo"
            )
              router-link(
                :to="makeLink(field.linkTo)"
              )
                | {{ fieldValue(field.name) }}

            span(
              v-else
              class="text--primary"
            )
              | {{ fieldValue(field.name) }}
</template>

<script>
import BasePaperPart from './BasePaperPart'

export default {
  extends: BasePaperPart,

  name: 'data-paper',

  computed: {
    fields () {
      if (!this.paper) return []

      if (this.paper.fields) {
        return Object.keys(this.paper.fields).map(name => ({
          name,
          ...this.paper.fields[name]
        }))
      }

      return []
    }
  },

  methods: {
    fieldValue (fieldName) {
      return this.paper.data[fieldName]
    },

    makeLink (rel) {
      var link = this.paper.links.filter(link => link.rel === rel)[0]
      if (!link) return ''

      var href = link.href
      if (!href) return ''
 
      var path = href.split('!/')[1]
      if (!path) return ''

      return `/Papers/${path}`
    }
  }
}
</script>