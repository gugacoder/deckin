<template lang="pug">
  v-app.home-view
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
          :indeterminate="busy"
        )

    the-app-menu(
      v-model="menu"
    )

    the-content(
      prominent
    )
      v-col(
        class="text-center"
        cols="12"
      )
        span.title.font-weight-regular Bem-vindo ao &nbsp;
          app-title

      v-banner.py-8(
        v-if="busy"
      )
        v-avatar(
          slot="icon"
          size="40"
        )
          v-progress-circular(
            :size="40"
            color="primary"
            indeterminate
          )
        
        | Estamos checando a sanidade do sistema.

        br

        | Isso deve levar apenas um instante...

      v-banner.py-8(
        v-if="!busy && fault"
      )
        v-avatar(
          slot="icon"
          color="warning"
          size="40"
        )
          v-icon(
            color="white"
          )
            | mdi-wifi-strength-alert-outline
        
        p.font-weight-bold
          span O servidor não está disponível no momento. &nbsp;
            span.font-weight-light
              | Verifique se o dispositivo está conectado à rede
              | ou consulte o administrador do sistema.
        
        p.font-weight-light.text--secondary
          | Causa: {{ message || 'Desconhecida' }}

        template(
          v-slot:actions
        )
          v-btn(
            text
            color="primary"
            @click="ignite"
          )
            | Tentar novamente
</template>

<script>
import { BeforeInstallPromptEvent } from 'vue-pwa-install';
import TheHeader from '@/components/layout/TheHeader.vue'
import TheContent from '@/components/layout/TheContent.vue'
import TheFooter from '@/components/layout/TheFooter.vue'
import TheAppMenu from '@/components/layout/TheAppMenu.vue'
import AppTitle from '@/components/layout/AppTitle.vue'
import delay from 'delay'

export default {
  name: 'home-view',

  deferredPrompt: BeforeInstallPromptEvent,

  components: {
    TheHeader,
    TheContent,
    TheFooter,
    TheAppMenu,
    AppTitle,
  },

  data: () => ({
    busy: true,
    fault: null,
    menu: false,
  }),

  mounted() {
    this.ignite()
  },

  computed: {
    message () {
      let fault = this.fault ?? {}
      return (fault.data && fault.data.reason) || fault.description
    },
  },

  methods: {
    async ignite () {
      try {
        this.busy = true

        await delay(2000)

        let href = '/Api/1/Papers/Keep.Paper/System/Status'
        let data = {
          clientVersion: '0.1.0'
        }

        let paper = await this.$browser.request(href, data)

        // O servidor é considerado válido se retornar uma resposta com pelo
        // menos a estrutura abaixo:
        //   {
        //     kind: "info",
        //     data: {
        //       serverVersion: "0.1.0"
        //     }
        //   };
        let ok = (paper.kind === 'info') && paper.data.serverVersion
        if (ok) {
          this.$router.push('/Home')
        } else {
          this.fault = paper
        }

      } catch(error) {
        this.fault = error
      } finally {
        this.busy = false
      }
    },
  },
}
</script>