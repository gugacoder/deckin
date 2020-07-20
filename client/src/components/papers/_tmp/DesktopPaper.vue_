<template lang="pug">
  div.desktop-paper
    v-btn(
      v-for="item in items"
      :key="item.uid"
    )
      | {{ item.view.title }}

</template>

<script>
import BasePaperPart from './BasePaperPart'

export default {
  extends: BasePaperPart,
  
  name: 'desktop-paper',

  computed: {
    items () {
      return this.paper.embedded.filter(item => item.rel === 'item')
    }
  }
}
</script>