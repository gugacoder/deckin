<template lang="pug"> 
  v-app.home-paper.x-container
    the-header(
      :prominent="prominent"
      :catalog="catalogName"
      :caption="title"
      :noTitle="!prominent"
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
      @refresh="() => { /* Nada a fazer por enquanto */ }"
    )

    the-content(
      :prominent="prominent"
    ) 
      the-alert(
        v-model="alert"
      )

      v-container(
        fluid
      )
          
      v-expansion-panels(
        flat
        style="z-index: 0"
      )
        v-expansion-panel
          v-expansion-panel-header
            span.font-weight-thin Código Fonte
            
          v-expansion-panel-content
            small
              pre.font-weight-thin {{ paper }}
</template>

<style scoped>
.x-container {
  display: block;
}
</style>

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

  name: 'home-paper',

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
    alert: {
      type: null,     // String: info | success | warning | error,
      message: null,  // String
      detail: null    // String | Array
    },
  }),

  computed: {
    prominent () {
      return this.paper.meta.prominent
    },
  },

  created () {
    this.onPaperChanged()
  },

  watch: {
    'content.paper': 'onPaperChanged'
  },

  methods: {
    onPaperChanged () {
      // Auto show menu
      console.log('here')
      this.menu = true //this.paper.meta.menu
    }
  },
}
</script>
