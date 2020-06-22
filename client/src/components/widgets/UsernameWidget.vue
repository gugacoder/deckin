<template lang="pug">
  v-text-field(
    autocomplete="username"
    v-model="value"
    :label="title"
    :rules="rules"
    :required="required"
    :hint="hint"
    dense
    ref="widget"
  )
</template>

<script>
export default {
  name: 'username-widget',

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
  },

  methods: {
    focus () {
      this.$refs.widget.focus()
    }
  }
}
</script>