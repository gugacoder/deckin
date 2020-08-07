<template lang="pug">
  v-app.fault-paper
    the-header(
      prominent
      @menuClick="menu = !menu"
    )

    the-footer

    the-app-menu(
      v-model="menu"
    )

    the-content(
      prominent
    ) 
      the-alert(
        :type="fault === 'NotFound' ? 'warning' : 'error'"
        message="Não foi possível exibir os dados."
      )

      v-banner
        template(
          v-if="fault === 'NotFound'"
        )
          v-avatar(
            slot="icon"
            color="warning"
            size="40"
          )
            v-icon(
              color="white"
            )
              | mdi-ladybug
        
          p.font-weight-bold
            | Oops!
            br
            | A página não existe. &nbsp;

          p.font-weight-light
            | O servidor não pôde processar a requisição porque o endereço
            | solicitado pelo aplicativo não existe.

        template(
          v-else-if="fault === 'NetworkError'"
        )
          v-avatar(
            slot="icon"
            color="error"
            size="40"
          )
            v-icon(
              color="white"
            )
              | mdi-lan-disconnect
        
          p.font-weight-bold
            | Oops!
            br
            | Não foi possível acessar a rede. &nbsp;

          p.font-weight-light
            | Uma requisição de dados foi feita para o servidor mas a resposta
            | não chegou ou não foi compreendida.

          p.font-weight-light
            | Cheque sua conexão de rede e verifique se o servidor está online.

        template(
          v-else
        )
          v-avatar(
            slot="icon"
            color="error"
            size="40"
          )
            v-icon(
              color="white"
            )
              | mdi-ghost
        
          p.font-weight-bold
            | Oops!
            br
            | Alguma coisa não deu certo. &nbsp;

          p.font-weight-light
            | A tentativa de realizar o procedimento anterior produziur um
            | resultado inesperado.
          
        p.font-weight-light
          | Se o problema persistir consulte o administrador do sistema.

        small.font-weight-thin Causa: &nbsp;

          template(
            v-for="line in linesOfMessage"
          )
            span.font-weight-thin {{ line }}
            br
          
        template(
          v-slot:actions
        ) 
          v-btn(
            text
            color="primary"
            to="/Home"
          )
            | Voltar para o início
          
      v-expansion-panels(
        v-if="stackTrace"
        flat
      )
        v-expansion-panel
          v-expansion-panel-header
            span.font-weight-thin Detalhes técnicos:
            
          v-expansion-panel-content
            small.font-weight-thin {{ stackTrace }}
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

  name: 'fault-paper',

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

  computed: {
    fault () {
      return this.paper.data.fault
    },

    linesOfMessage () {
      let reason = this.paper.data.reason
      
      if (!reason)
        return []

      if (Array.isArray(reason))
        return reason

      return [ reason ]
    },

    stackTrace () {
      return this.paper.data.stackTrace
    },
  },
}
</script>