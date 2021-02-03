<template lang="pug">
  v-app-bar-nav-icon.the-app-menu-button.ml-0(
    icon
    @click.stop="e => $emit('input', !value)"
  )
</template>

<script>
export default {
  name: 'the-app-menu-button',

  props: {
    value: {
      type: Boolean,
    },
  },
}
</script>