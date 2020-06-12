<template lang="pug">
  div(
    class="invalid-paper"
  )
    p O conteúdo recebido do servidor não é suportado.
    pre {{ paper }}
</template>

<script>
export default {
  name: 'invalid-paper',

  props: [
    'catalogName',
    'paperName',
    'paperAction',
    'paperKeys',
    'paper'
  ],

  computed: {
    title () {
      if (this.paper.title) return this.paper.title

      var tokens = [
        this.paperName, 
        this.paperAction,
        ...this.paperKeys
      ]

      return tokens.join(' / ')
    }
  }
}
</script>