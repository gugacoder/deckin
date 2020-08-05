<template lang="pug"> 
  v-app.action-paper
    the-header(
      @menuClick="menu = !menu"
      :prominent="paper.view.design === 'login'"
    )
    
    the-footer
      v-btn(
        icon
        @click="$router.go()"
      )
        v-progress-circular(
          size="24"
          width="2"
          value="0"
          color="primary"
        )

    the-app-menu(
      v-model="menu"
    )

    the-content(
      prominent
    ) 
      the-alert(
        v-model="alert"
      )

      action-slice(
        v-bind="actionSlice"
        @paperReceived="paper => this.paper = paper"
        @cancel="cancel"
      )
      //-
              prepend-icon="mdi-account"
                    type="password"
</template>

<script>
import PaperBase from './-PaperBase.vue'
import TheHeader from '@/components/layout/TheHeader.vue'
import TheContent from '@/components/layout/TheContent.vue'
import TheFooter from '@/components/layout/TheFooter.vue'
import TheAppMenu from '@/components/layout/TheAppMenu.vue'
import TheAlert from '@/components/layout/TheAlert.vue'
import AppTitle from '@/components/layout/AppTitle.vue'
import '@/helpers/StringHelper.js'

export default {
  extends: PaperBase,

  name: 'action-paper',

  components: {
    TheHeader,
    TheContent,
    TheFooter,
    TheAppMenu,
    TheAlert,
    AppTitle,
  },

  data: () => ({
    menu: false,
    busy: false,
    alert: null,
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
    cancel () {
      this.$refs.form.reset()
      this.$router.back()
      this.$router.go()
    },
  },
}
</script>