<template lang="pug"> 
  v-app.action-paper
    the-header(
      :prominent="prominent"
      :catalog="catalogName"
      :caption="title"
    )
      template(
        slot="first"
      )
        the-app-menu-button(
          v-model="menu"
        )

      template(
        slot="right"
      )
        the-app-user-menu-button(
          v-model="userMenu"
        )

    the-app-menu(
      v-model="menu"
      :catalog="catalogName"
      :caption="title"
    )
    
    the-app-user-menu(
      v-model="userMenu"
    )
    
    the-footer(
      @refreshClick="() => { /* Nada a fazer por enquanto */ }"
    )

    the-content(
      :prominent="prominent"
    ) 
      the-alert(
        v-model="alert"
      )

      action-slice(
        v-bind="actionSlice"
        @paperReceived="paper => this.paper = paper"
        @cancel="cancel"
      )
</template>

<script>
import PaperBase from './-PaperBase.vue'
import TheHeader from '@/components/layout/TheHeader.vue'
import TheContent from '@/components/layout/TheContent.vue'
import TheFooter from '@/components/layout/TheFooter.vue'
import TheAppMenu from '@/components/layout/TheAppMenu.vue'
import TheAppMenuButton from '@/components/layout/TheAppMenuButton.vue'
import TheAppUserMenu from '@/components/layout/TheAppUserMenu.vue'
import TheAppUserMenuButton from '@/components/layout/TheAppUserMenuButton.vue'
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
    TheAppMenuButton,
    TheAppUserMenu,
    TheAppUserMenuButton,
    TheAlert,
    AppTitle,
  },

  data: () => ({
    menu: false,
    userMenu: false,
    busy: false,
    alert: null,
  }),

  computed: {
    prominent () {
      return this.paper.view.design === 'login'
    },
    
    actionSlice () {
      return {
        paper: this.paper,
        actionName: null,   // O próprio paper é a ação
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