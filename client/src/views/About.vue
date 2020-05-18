<template>
  <v-app class="about">
    <the-header
      :title="$options.name"
    >
    </the-header>

    <the-content>
      <cards-paper/>
    </the-content>
  </v-app>
</template>

<script>
import TheHeader from '@/components/layout/TheHeader.vue'
import TheContent from '@/components/layout/TheContent.vue'
import CardsPaper from '@/components/papers/CardsPaper.vue'

export default {
  name: 'About',
  components: {
    TheHeader,
    TheContent,

    CardsPaper,
  }
}
</script>
