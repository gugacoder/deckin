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
      required: false
    },
    paperName: {
      type: String,
      required: false
    },
    actionName: {
      type: String,
      required: true
    },
    actionKeys: {
      type: String,
      required: false
    },
    paper: {
      type: Object,
      required: true
    },
  }
}
</script>