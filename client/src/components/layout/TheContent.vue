<template lang="pug">
  v-sheet#x-content.the-content.overflow-y-auto(
    :max-height="windowSize.y"
    v-resize="onResize"
  )
    //- 128: prominent header height
    //-  56: header height
    //-  48: footer height
    v-sheet(
      :min-height="windowSize.y - (prominent ? 128 : 56) - 48"
      :style="`margin-top: ${prominent ? 128 : 56}px; margin-bottom: 48px;`"
    )
      v-container
        slot
</template>

<style scoped>
</style>

<script>
export default {
  name: 'the-content',

  props: {
    prominent: {
      type: Boolean,
      required: false
    }
  },

  data: () => ({
    windowSize: {
      x: 0,
      y: 0
    },
  }),

  computed: {
  },

  mounted () {
    this.onResize()
  },

  methods: {
    onResize () {
      this.windowSize = { x: window.innerWidth, y: window.innerHeight }
    },
  },
}
</script>