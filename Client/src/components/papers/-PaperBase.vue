<template lang="pug">
  div
</template>

<script>
export default {
  props: {
    catalogName: {
      type: String,
      required: true
    },

    paperName: {
      type: String,
      required: true
    },

    actionName: {
      type: String,
      required: true
    },

    actionKeys: {
      type: String,
      required: false
    },

    value: {
      type: Object,     // type: Entity
      required: true
    }
  },

  computed: {
    parameters () {
      return {
        catalogName: this.catalogName,
        paperName: this.paperName,
        actionName: this.actionName,
        actionKeys: this.actionKeys,
        content: this.content
      }
    },

    paper: {
      get () {
        return this.value
      },
      set (value) {
        this.$emit('input', value)
      }
    },
    
    actionArgs () {
      return (this.paperKeys || '').split(';')
    },

    type () {
      return this.paper.props['@type']
    },

    title () {
      return this.paper.props.title
    },
  },
}
</script>