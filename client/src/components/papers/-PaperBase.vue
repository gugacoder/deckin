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
    // content deve ser:
    // {
    //   paper: ENTITY,   // Requerido
    //   alert: {
    //     type: ...,
    //     message: ...,
    //     detail: ...,
    //     fault: ...
    //   }
    // }
    content: {
      type: Object,
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

    paper () {
      return this.content.paper
    },

    title () {
      return this.paper.view.title
    },
    
    alert () {
      return this.content.alert
    }
  },

  methods: {
    setPaper (value) {
      this.content.paper = value
    },

    setAlert (value) {
      if (!value) value = {}
      this.content.alert.type = value.type
      this.content.alert.message = value.message
      this.content.alert.detail = value.detail
      this.content.alert.fault = value.fault
    }
  }
}
</script>