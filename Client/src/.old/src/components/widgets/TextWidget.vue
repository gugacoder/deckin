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
    :type="type"
    :autocomplete="autoComplete"
    :prepend-icon="icon"
    :readonly="readOnly"
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

  computed: {
    type () {
      return this.field.props.password ? 'password' : undefined
    },

    icon () {
      return this.field.props.icon
    },

    autoComplete () {
      return this.field.props.autoComplete
    },
  },

  methods: {
    focus () {
      this.$refs.widget.focus()
    },
  }
}
</script>