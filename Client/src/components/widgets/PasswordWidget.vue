<template lang="pug">
  v-text-field.password-widget(
    type="password"
    autocomplete="current-password"
    v-show="!hidden"
    v-model="value"
    :class="extent"
    :label="title"
    :rules="rules"
    :required="required"
    :hint="hint"
    prepend-icon="mdi-lock"
    dense
  )
</template>

<script>
import WidgetBase from './-WidgetBase.vue'

export default {
  extends: WidgetBase,
  
  name: 'password-widget',
}
</script>