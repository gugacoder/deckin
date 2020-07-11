<template lang="pug">
  v-footer(
    app
    fixed
    elevation="3"
    color="white"
  )
    small.font-weight-thin © 2020 KeepCoding™

    small
      slot(
        name="left"
      )

    v-spacer

    small
      slot

    small
      slot(
        name="right"
      )

</template>

<script>
//import Vue from 'vue'
import { mapState } from 'vuex'
import BasePaperPart from '../BasePaperPart'
import '@/helpers/StringHelper'

export default {
  extends: BasePaperPart,

  name: 'the-paper-footer',

  computed: {
    ...mapState('system', [
      'identity'
    ]),
  },
}
</script>