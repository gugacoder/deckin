<template lang="pug">
  v-text-field(
    autocomplete="username"
    v-model="value"
    :label="title"
    :rules="rules"
    :required="required"
    :hint="hint"
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
        return this.field.view.value
      },
      set (value) {
        this.field.view.value = value
      }
    },

    kind () {
      return this.field.kind
    },

    title () {
      return this.field.view.title
    },

    required () {
      return this.field.view.required
    },

    hint () {
      return this.field.view.hint
    },

    rules () {
      return [
        (v) => !this.required || !!v || 'Requerido'
      ]
    }
  }
}
</script>