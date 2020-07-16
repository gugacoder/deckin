<template lang="pug">
  v-skeleton-loader.skeleton-widget(
    type="list-item"
    class="mx-auto"
  )
</template>

<script>
export default {
  name: 'skeleton-widget',

  props: {
    extent: {
      type: Number,
      required: false
    },
  },
}
</script>