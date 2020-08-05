<template lang="pug">
  v-app.login-paper
    the-header(
      prominent
      @menuClick="menu = !menu"
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
      v-container(
        class="fill-height"
        fluid
      )
        v-row(
          align="center"
          justify="center"
        )
          v-col(
            cols="12"
            sm="8"
            md="4"
          )
            v-form
              v-card(
                flat
              )
                v-card-text
                  v-text-field(
                    label="Usuário"
                    name="login"
                    prepend-icon="mdi-account"
                    type="text"
                  )

                  v-text-field(
                    id="password"
                    label="Senha"
                    name="password"
                    prepend-icon="mdi-lock"
                    type="password"
                  )

                v-card-actions.pr-4
                  v-spacer

                  v-btn(
                    color="primary"
                  )
                    | Entrar
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

  name: 'login-paper',

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
  }),
}
</script>