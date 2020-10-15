<template lang="pug">
  v-app.action-view
    v-app-bar(
      app
      dark
      color="primary"
    )
      big
        span.font-weight-light 
        span.font-weight-bold Demo
        span.font-weight-light App
          sup
            small.font-weight-light 

    v-main
      v-container(
        flex
      )
        v-card
          v-card-title(
            class="headline"
          )
            span Workspace
              sup
                small.font-weight-thin ( {{ workspacePath }} )

          v-card-subtitle
            pre {{ workspaceInstance }}

          v-divider.mx-4

          v-card-title(
            class="headline"
          )
            span Action
              sup
                small.font-weight-thin ( {{ actionPath }} )

          v-card-subtitle
            pre {{ actionInstance }}

          //-
            v-divider.mx-4

            v-card-actions
              v-btn(
                color="primary"
                @click="change"
              )
                | Change
</template>

<script>
import { createNamespacedHelpers } from 'vuex'

const { mapActions } = createNamespacedHelpers('entity')

export default {
  name: 'action-view',

  props:
  {
    workspace: {
      type: String,
      required: true
    },
    action: {
      type: String,
      required: true
    },
  },

  data: () => ({
  }),

  computed: {
    workspacePath () {
      return '/Api/1/Papers/' + this.workspace
    },

    actionPath () {
      return '/Api/1/Papers/' + this.action
    },

    workspaceInstance () {
      return this.$store.state.entity[this.workspacePath]
    },

    actionInstance () {
      return this.$store.state.entity[this.actionPath]
    },
  },

  created () {
    this.fetchData();
  },

  watch: {
    '$route': 'fetchData',
  },

  methods: {
    ...mapActions([
      'get'
    ]),

    async fetchData () {
      await this.get(this.workspacePath)
      await this.get(this.actionPath)
    },
  },
}
</script>