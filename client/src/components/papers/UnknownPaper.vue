<template lang="pug">
  div(
    class="unknown-paper"
  )
    p O conteúdo recebido do servidor não é suportado.
    pre {{ paper }}
</template>

<script>
export default {
  name: 'unknown-paper',

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
      console.log('content', this.content)
      return this.content.paper
    },
  }
}
</script>