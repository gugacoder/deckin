<template lang="pug">
  v-text-field.text-widget(
    v-show="!hidden"
    v-model="value"
    :class="`extent-${extent}`"
    :label="title"
    :rules="rules"
    :required="required"
    :hint="hint"
    dense
    ref="widget"
  )
</template>

<script>
import WidgetBase from './-WidgetBase.vue'

export default {
  extends: WidgetBase,
  
  name: 'text-widget',

  methods: {
    focus () {
      this.$refs.widget.focus()
    },
  }
}
</script>