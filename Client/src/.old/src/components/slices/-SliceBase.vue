<template lang="pug">
  div
</template>

<script>

export default {
  props: {
    paper: {
      type: Object,   // Paper
      required: true
    },
  },

  computed: {
    parameters () {
      return {
        paper: this.paper
      }
    },

    title () {
      return this.paper.props.title
    },
  },
}
</script>