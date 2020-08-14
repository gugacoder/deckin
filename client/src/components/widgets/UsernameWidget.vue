<template lang="pug">
  v-text-field.username-widget(
    autocomplete="username"
    v-show="!hidden"
    v-model="value"
    :class="extent"
    :label="title"
    :rules="rules"
    :required="required"
    :hint="hint"
    dense
    prepend-icon="mdi-account"
    ref="widget"
  )
</template>

<script>
import WidgetBase from './-WidgetBase.vue'

export default {
  extends: WidgetBase,
  
  name: 'username-widget',

  methods: {
    focus () {
      this.$refs.widget.focus()
    }
  }
}
</script>