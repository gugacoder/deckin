<template lang="pug">
</template>

<style scoped lang="scss">
$base-width-xs: 160px;
$base-width: 130px;
$base-margin: 12px;

@for $i from 1 to 5 {
  .x-extent-#{$i}-xs {
    min-width: $base-width-xs;
    max-width: ($base-width-xs * $i) + ($base-margin * ($i - 1));
    width: ($base-width-xs * $i) + ($base-margin * ($i - 1));
    margin: 0;
    margin-bottom: $base-margin;
    margin-right: $base-margin;
  }
}

@for $i from 1 to 5 {
  .x-extent-#{$i} {
    min-width: $base-width;
    max-width: ($base-width * $i) + ($base-margin * ($i - 1));
    width: ($base-width * $i) + ($base-margin * ($i - 1));
    margin: 0;
    margin-bottom: $base-margin;
    margin-right: $base-margin;
  }
}
</style>

<script>
export default {
  props: {
    paper: {
      type: Object, // Paper
      required: true
    },
    fieldName: {
      type: String,
      required: true
    },
  },

  data: () => ({
    alert: {
      type: 'warning',  // Um de 'info', 'success', 'warning', 'error'
      message: null,
      detail: null,     // Um texto ou linhas de texto de detalhe
    },
  }),

  computed: {
    parameters () {
      return {
        paper: this.paper,
        fieldName: this.fieldName,
      }
    },

    field () {
      let fields = this.paper.fields
      let field = fields.filter(field => field.view.name === this.fieldName)[0]
      return field
    },

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

    hidden () {
      return !!this.field.view.hidden
    },

    rules () {
      return [
        (v) => !this.required || !!v || 'Requerido'
      ]
    },

    extent () {
      let factor = this.field.view.extent || this.defaultExtent || 2
      let suffix = this.$isMobile ? '-xs' : ''
      let extent = `x-extent-${factor}${suffix}`
      return extent
    },

    errorMessages () {
      return this.alert && this.alert.message
    },
  },
}
</script>