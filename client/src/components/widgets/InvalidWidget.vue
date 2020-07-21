<template lang="pug">
  v-text-field.invalid-widget(
    v-show="!hidden"
    v-model="value"
    :label="title"
    :hint="hint"
    :class="`extent-${extent}`"
    disabled
    dense
  )
</template>

<script>
import WidgetBase from './-WidgetBase.vue'

export default {
  extends: WidgetBase,
  
  name: 'invalid-widget',
}
</script>