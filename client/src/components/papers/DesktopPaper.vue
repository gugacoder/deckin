<template lang="pug">
  div.desktop-paper
    v-btn(
      v-for="item in items"
    )
      | {{ item.view.title }}

</template>

<script>
import BasePaper from './BasePaper.vue'

export default {
  extends: BasePaper,
  
  name: 'desktop-paper',

  computed: {
    items () {
      return this.paper.embedded.filter(item => item.rel === 'item')
    }
  }
}
</script>