<template lang="pug">
  v-text-field(
    autocomplete="current-password"
    v-show="!hidden"
    v-model="value"
    :label="title"
    :hint="hint"
    enabled="false"
    dense
  )
</template>

<script>
export default {
  name: 'password-widget',

  props: [
    'field'
  ],

  computed: {
    name () {
      return this.field.data.name
    },

    value: {
      get () {
        return this.field.data.value
      },
      set (value) {
        this.field.data.value = value
      }
    },

    title () {
      return this.field.view.title
    },

    hint () {
      return this.field.view.hint
    },

    hidden () {
      return !!this.field.view.hidden
    }
  }
}
</script>