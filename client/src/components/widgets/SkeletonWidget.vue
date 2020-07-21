<template lang="pug">
  v-skeleton-loader.skeleton-widget(
    type="list-item"
    class="mx-auto"
    :class="`extent-${extent}`"
  )
</template>

<script>
import WidgetBase from './-WidgetBase.vue'

export default {
  extends: WidgetBase,
  
  name: 'skeleton-widget',

  props: {
    extent: {
      type: Number,
      required: false
    },
  },
}
</script>