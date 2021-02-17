<template lang="pug">
v-container(:class="name")
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
import TypedVue from '@/typed-vue'
import { Component, Prop } from "vue-property-decorator";
// import { browser } from '@/browser'

@Component
export default class Paper extends TypedVue {
  name = "splash";

  @Prop()
  private paper!: string;

  mounted() {
    //this.$store.getters
  }
}
</script>
