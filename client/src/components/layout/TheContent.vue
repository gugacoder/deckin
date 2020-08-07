<template lang="pug">
  v-sheet#x-content.the-content.overflow-y-auto(
    :max-height="windowSize.y"
    v-resize="onResize"
  )
    //- 128: prominent header height
    //-  56: header height
    //-  48: footer height
    v-sheet(
      :min-height="windowSize.y - (marginTop) - 48"
      :style="`margin-top: ${marginTop}px; margin-bottom: 48px;`"
    )
      v-container(
        :class="noMargins ? 'px-0' : null"
        :fluid="fluid"
      )
        slot
</template>

<style scoped>
</style>

<script>
export default {
  name: 'the-content',

  props: {
    prominent: {
      type: Boolean
    },

    noMargins: {
      type: Boolean
    },

    fluid: {
      type: Boolean
    }
  },

  data: () => ({
    windowSize: {
      x: 0,
      y: 0
    },
  }),

  computed: {
    marginTop () {
      return this.prominent ? 128 : 80
    },
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