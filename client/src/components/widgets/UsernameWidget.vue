<template lang="pug">
  v-text-field(
    autocomplete="username"
    v-model="value"
    :label="label"
    :rules="rules"
    :required="required"
    :hint="hint"
    outlined
    dense
  )
</template>

<script>
export default {
  name: 'username-widget',

  props: [
    'field'
  ],

  computed: {
    value: {
      get () {
        return this.field.value
      },
      set (value) {
        this.field.value = value
      }
    },

    kind () {
      return this.field.kind
    },

    label () {
      return this.field.label
    },

    required () {
      return this.field.required
    },

    hint () {
      return this.field.hint
    },

    rules () {
      return [
        (v) => !this.required || !!v || 'Requerido'
      ]
    }
  }
}
</script>