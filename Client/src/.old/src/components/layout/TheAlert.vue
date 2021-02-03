<template lang="pug">
  v-alert.the-alert(
    v-model="alertVisible"
    :type="alertType"
    :dismissible="alertDismissible"
    :icon="icon"
    dense
    text
    border="left"
    transition="fade-transition"
    @input="$listeners.close ? $emit('close') : $emit('input', null)"
  )
    template(
      v-for="line in alertMessages"
    )
      span.font-weight-medium {{ line }}
      br
  
    template(
      v-for="line in alertDetails"
    )
      span.font-weight-light {{ line }}
      br
</template>

<script>
export default {
  name: 'the-alert',

  props: {
    // Um objeto contendo as propriedades do alerta:
    // {
    //   type: String,            // Um de 'info', 'success', 'warning', 'error'
    //   message: String,
    //   detail: String | Array   // Um texto ou linhas de texto de detalhe
    // }
    value: {
      type: Object,
      // validator (value) {
      //   console.log({value})
      //   // Pelo menos a mensagem deve ser indicada.
      //   return !value || !!value.message
      // }
    },

    type: {
      type: String,
      // NOTE: Validador desativado porque não queremos ser tão restritivos assim.
      // validator: (value) => {
      //   return [ 'info', 'success', 'warning', 'error' ].indexOf(value) !== -1
      // }
    },

    message: {
      // Um texto ou várias linhas de texto.
      type: [ String, Array ],
    },

    detail: {
      // Um texto ou várias linhas de texto.
      type: [ String, Array ],
    },

    dismissible: {
      type: Boolean
    },

    icon: {
      type: String
    }
  },

  data: () => ({
  }),

  computed: {
    alertVisible: {
      get () {
        return !!(this.message || (this.value && this.value.message))
      },
      set () {
        // Nada a fazer.
        // A visibilidade é controlada pelos campos "value" e "message".
      }
    },

    alertDismissible () {
      return this.dismissible || !!(this.value || this.$listeners.close)
    },

    alertType () {
      let type = this.type || (this.value && this.value.type)
      let isValid = [ 'success', 'warning', 'error' ].indexOf(type) !== -1
      return isValid ? type : 'info'
    },

    alertMessages () {
      let message = this.message || (this.value && this.value.message)

      if (!message)
        return []

      if (Array.isArray(message))
        return message

      return [ message ]
    },

    alertDetails () {
      let detail = this.detail || (this.value && this.value.detail)

      if (!detail)
        return []

      if (Array.isArray(detail))
        return detail

      return [ detail ]
    }
  }
}
</script>