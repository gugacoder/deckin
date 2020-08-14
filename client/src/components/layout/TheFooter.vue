<template lang="pug">
  v-footer.the-footer.px-1(
    app
  )
    slot(
      name="left"
    )

    v-spacer
    
    slot
    
    v-btn(
      icon
      @click.stop="refresh"
    )
      slot(
        name="refresh"
      )
        v-progress-circular(
          size="24"
          width="4"
          value="0"
          color="primary"
          :indeterminate="busy"
        )

    slot(
      name="right"
    )

</template>

<script>
export default {
  name: 'the-footer',

  props: {
    busy: {
      type: Boolean
    }
  },

  methods: {
    refresh () {
      if (this.$listeners.refresh) {
        this.$emit('refresh', () => this.$router.go())
      } else {
        this.$router.go()
      }
    }
  }
}
</script>
