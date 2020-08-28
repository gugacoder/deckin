<template lang="pug">
  v-text-field.text-widget(
    v-show="!hidden"
    v-model="value"
    :class="extent"
    :label="title"
    :rules="rules"
    :required="required"
    :hint="hint"
    :error-messages="errorMessages"
    dense
    ref="widget"
  )
</template>

<script>
import WidgetBase from './-WidgetBase.vue'

export default {
  extends: WidgetBase,
  
  name: 'text-widget',

  data: () => ({
    defaultExtent: 2,
  }),

  methods: {
    focus () {
      this.$refs.widget.focus()
    },
  }
}
</script>