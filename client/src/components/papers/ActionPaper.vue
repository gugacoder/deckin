<template lang="pug"> 
  v-container(
  )
    the-paper-header(
      app
      v-bind="parameters"
    )
    
    the-paper-footer(
      v-bind="parameters"
    )

    action-slice(
      v-bind="actionSlice"
      @paperReceived="setPaper"
      @cancel="cancel"
    )
</template>

<script>
import PaperBase from './-PaperBase.vue'
import ThePaperHeader from './parts/ThePaperHeader.vue'
import ThePaperFooter from './parts/ThePaperFooter.vue'
import '@/helpers/StringHelper.js'

export default {
  extends: PaperBase,

  name: 'action-paper',

  components: {
    'the-paper-header': ThePaperHeader,
    'the-paper-footer': ThePaperFooter,
  },

  data: () => ({
    batteryLevel: 0
  }),

  computed: {
    actionSlice () {
      return {
        paper: this.paper,
        actionName: null,   // O próprio paper é a áção
      }
    },
  },

  methods: {
    submit () {
      alert('hi')
    },

    cancel () {
      this.$refs.form.reset()
      this.$router.back()
      this.$router.go()
    },
  },
}
</script>