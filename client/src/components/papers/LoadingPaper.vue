<template lang="pug">
  v-app.loading-paper
    the-header(
      prominent
      @menuClick="menu = !menu"
    )

    the-footer(
      :busy="true"
    )

    the-app-menu(
      v-model="menu"
    )

    the-content(
      prominent
    )
      v-row.mt-8(
        class="fill-height"
        align-content="center"
        justify="center"
      )
        v-col(
          class="text-center"
          cols="12"
        )
          span.font-weight-regular Carregando dados...

        v-col(
          cols="6"
        )
          v-progress-linear(
            size="24"
            height="6"
            value="0"
            color="primary"
            indeterminate
            rounded
          )
</template>

<script>
import PaperBase from './-PaperBase.vue'
import TheHeader from '@/components/layout/TheHeader.vue'
import TheContent from '@/components/layout/TheContent.vue'
import TheFooter from '@/components/layout/TheFooter.vue'
import TheAppMenu from '@/components/layout/TheAppMenu.vue'
import TheAlert from '@/components/layout/TheAlert.vue'
import AppTitle from '@/components/layout/AppTitle.vue'

export default {
  extends: PaperBase,

  name: 'loading-paper',

  components: {
    TheHeader,
    TheContent,
    TheFooter,
    TheAppMenu,
    TheAlert,
    AppTitle,
  },

  data: () => ({
    busy: false,
    menu: false,
  }),
}
</script>