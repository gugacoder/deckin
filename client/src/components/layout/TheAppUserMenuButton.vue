<template lang="pug">
  v-btn.the-app-user-menu-button(
    icon
    @click.stop="e => $emit('input', !value)"
  )
    v-icon {{ identity ? 'mdi-account-circle' : 'mdi-account-circle-outline' }}
</template>

<style scoped>
</style>

<script>
import { mapState } from 'vuex'

export default {
  name: 'the-app-user-menu-button',

  props: {
    value: {
      type: Boolean,
    },
  },

  computed: {
    ...mapState('system', [
      'identity',
    ]),
  },
}
</script>