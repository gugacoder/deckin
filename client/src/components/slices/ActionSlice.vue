<template lang="pug">
  div.action-slice.flex-container.pl-4.pt-4
    div.stretch(
      v-for="field in fields"
      :key="field.view.name"
      :class="`extent-1`"
    )
      password-widget.stretch(
        :field="field"
      )
</template>

<style scoped lang="scss">
$base-width: 130px;
$base-margin: 12px;

@for $i from 1 to 5 {
  .extent-#{$i} {
    min-width: $base-width;
    max-width: ($base-width * $i) + ($base-margin * ($i - 1));
    margin: 0;
    margin-bottom: $base-margin;
    margin-right: $base-margin;
  }
}

.flex-container {
  display: inline-flex;
  flex-wrap: wrap;
  width: 100%;
}

.stretch {
  width: 100%;
}
</style>

<script>
export default {
  name: 'action-slice',

  props: {
    catalogName: {
      type: String,
      required: true
    },
    paperName: {
      type: String,
      required: true
    },
    actionName: {
      type: String,
      required: true
    },
    actionKeys: {
      type: String,
      required: false
    },
    // content deve ser:
    // {
    //   paper: { ... }
    // }
    content: {
      type: Object,
      required: true
    },
    // action deve ser um paper:
    // {
    //   kind: 'action',
    //   ...
    // }
    action: {
      type: Object,
      required: true
    }
  },

  data: () => ({
    extents: [ 1, 1, 1, 1, 1, 4, 2, 3, 1, 3 ]
  }),

  computed: {
    parameters () {
      return {
        catalogName: this.catalogName,
        paperName: this.paperName,
        actionName: this.actionName,
        actionKeys: this.actionKeys,
        content: this.content
      }
    },

    paper () {
      return this.content.paper
    },

    title () {
      return this.paper.view.title
    },
    
    alert () {
      return this.content.alert
    },

    fields () {
      return this.paper.fields.filter(field => !field.view.hidden)
    }
  },

  methods: {
    setPaper (value) {
      this.content.paper = value
    },

    setAlert (value) {
      this.content.alert = value
    }
  }
}
</script>