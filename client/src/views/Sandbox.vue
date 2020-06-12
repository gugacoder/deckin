<template lang="pug">
  v-container.sandbox(
    id="sandbox-content"
  )
    pre
      | {{ sanitizedEntity }}
</template>

<script>
import { sanitizeEntity } from "@/helpers/EntityOperations.js";

export default {
  name: 'sandbox',

  data: () => ({
    source: {
      embedded: [
        {}
      ],
      kind: "action",
      view: {
        name: "default",
        title: "Credenciais de Usuário"
      },
      fields: {
        username: {
          kind: "username",
          view: {
            title: "Usuário",
            required: true
          }
        },
        password: {
          kind: "password",
          view: {
            title: "Senha",
            required: true
          }
        }
      },
      actions: {
        save: {
          fields: {
            id: {}
          }
        }
      },
      /*
      links: [{
          rel: "self",
          href: "/!/Keep.Paper/Login/Index"
        },
        {
          rel: "action",
          href: "/!/Keep.Paper/Login/Authenticate"
        }
      ]
      */
      links: {
        self: {
          href: "/!/Keep.Paper/Login/Index"
        },
        action: {
          href: "/!/Keep.Paper/Login/Authenticate"
        }
      }
    }
  }),

  computed: {
    sanitizedEntity () {
      return sanitizeEntity(this.source)
    }
  }
}
</script>