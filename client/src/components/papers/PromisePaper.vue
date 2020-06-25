<template lang="pug">
  div(
    class="data-paper"
  )
    v-card(
      class="mx-auto"
    )
      v-card-title
        | {{ title }} | {{ actionName }}

      v-card-text
        v-btn(
          class="mr-2"
          color="primary"
          @click="$router.go()"
        )
          | Recarregar esta página

</template>

<script>
export default {
  name: 'promise-paper',

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
  },

  computed: {
    title () {
      return (this.paper ? this.paper.view.title : null) || 'Unnamed'
    }
  }
}
</script>