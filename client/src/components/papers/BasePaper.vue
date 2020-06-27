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
    content: {
      type: Object,
      required: true
    }
  },

  computed: {
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
      this.content.alert = value
    }
  }
}
</script>