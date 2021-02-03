<template lang="pug">
div.paper
  h1 {{ workspace }} / {{ paper }}
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component
export default class Paper extends Vue {
  
  @Prop()
  private workspace!: string;

  @Prop()
  private paper!: string;
}
</script>
