<template lang="pug">
  v-app.unknown-paper
    the-header(
      prominent
      noTitle
      @menu="menu = !menu"
    )

    the-footer

    the-app-menu(
      v-model="menu"
    )

    the-content(
      prominent
    ) 
      the-alert(
        type="warning"
        message="Não foi possível exibir os dados."
      )

      v-banner
        v-avatar(
          slot="icon"
          color="warning"
          size="40"
        )
          v-icon(
            color="white"
          )
            | mdi-file-alert
        
        p.font-weight-bold
          span O conteúdo recebido do servidor não é suportado. &nbsp;
        
        p.font-weight-light
          | Existe um problema com o servidor. Uma requisição foi realizada
          | mas a resposta recebida é incompatível com esta versão do
          | sistema.
          
        p.font-weight-light
          | Entre em contato com o administrador do sistema se o problema
          | persistir.
          
        sup.font-weight-thin {{ $route.path }} ( design: {{ paper.view.design || paper.kind }} )
      
        template(
          v-slot:actions
        )
          v-btn(
            text
            color="primary"
            to="/Home"
          )
            | Voltar para o início
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

  name: 'unknown-paper',

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
