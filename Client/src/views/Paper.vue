<template lang="pug">
v-container.paper
  v-row.text-center
    v-col(cols="12")
      v-col.mb-4
        h1.headline.font-weight-bold.mb-3
          | Bem-vindo!
        p.subheading.font-weight-regular
          | Carregando os recursos do sistema...
        small.font-weight-thin.mb-3
          | ≪ AppSuite 0.1-alfa ≫
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class Paper extends Vue {
  name = "Paper";

  @Prop()
  private paper!: string;
}
</script>
