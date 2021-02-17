<template lang="pug">
v-app
  v-app-bar(app, color="primary", dark)
    big
      big Director
        strong Web
        span.font-weight-light.mb-3 &nbsp; {{ currentRouteName }}
  v-main
    router-view
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class Paper extends Vue {
  name = "App";

  @Prop()
  private paper!: string;

  get currentRouteName() {
    return this.$route.params.paper ? `/${this.$route.params.paper}` : "";
  }
}
</script>
