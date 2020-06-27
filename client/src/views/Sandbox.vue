<template lang="pug">
  v-container.sandbox(
    id="sandbox-content"
  )
    v-btn(
      color="primary"
      @click="go()"
    )
      | GO

    h1 {{ message }}
</template>

<script>

function guid() {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
    return v.toString(16);
  });
}

export default {
  name: 'sandbox',

  data: () => ({
    message: '...'
  }),

  computed: {
  },

  methods: {
    go () {
      this.$router.push(`/Sandbox/${guid()}`).then(() => {
        this.message = 'Tananana'
      })
    }
  }
}
</script>