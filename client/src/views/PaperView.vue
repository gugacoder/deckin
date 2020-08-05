<template lang="pug">
  div.paper-view
    component(
      v-bind="paperComponent"
      v-model="paper"
    )
</template>

<style scoped>
.custom-title a {
  text-transform: uppercase;
  text-decoration: none;
  color: var(--v-secondary-base)
}

.custom-title a:hover {
  text-transform: uppercase;
  text-decoration: none;
  color: lighten( red, 50% );
}
</style>

<script>
import Vue from 'vue'
import { unknownPaper, canonifyPaper } from '@/helpers/PaperHelper'
import '@/helpers/StringHelper'

const nullPaper = canonifyPaper({})

export default {
  name: 'paper-view',

  props:
  {
    catalogName: {
      type: String,
      required: true
    },
    paperName: {
      type: String,
      required: true
    },
    actionName: {
      type: String,
      required: true
    },
    actionKeys: {
      type: String,
      required: false
    }
  },

  data: () => ({
    preFetchedPaper: null,
    paper: nullPaper,
  }),

  computed: {
    busy () {
      return this.paper === nullPaper
    },

    paperComponent () {
      let name
      
      // Decidindo o nome do component base do paper...
      //
      if (this.busy) {
        name = 'loading-paper'
      } else {
        let paper = this.paper

        let design = (paper && (paper.view.design || paper.kind)) || 'unknown'
        let designName = design.toHyphenCase()
        
        // Páginas ainda não implementadas
        if (designName === 'login') designName = 'action'

        name = `${designName}-paper`
        if (!Vue.options.components[name]) {
          name = 'unknown-paper'
        }
      }

      return {
        is: name,
        catalogName: this.catalogName,
        paperName: this.paperName,
        actionName: this.actionName,
        actionKeys: this.actionKeys,
      }
    },
  },

  created () {
    this.fetchData();
  },

  watch: {
    '$route': 'fetchData',
  },

  methods: {
    async fetchData () {
      try {
        let targetPaper = this.preFetchedPaper

        if (!targetPaper) {
          let href = this.$browser.href(this)
          let data = this.$route.query
          targetPaper = await this.$browser.request(href, data) || unknownPaper
        }

        let targetSelf = targetPaper.getLink('self') || { href: this.href }
        let targetPath = this.$browser.routeFor(targetSelf.href)
        if (targetPath !== this.$route.path) {
          this.fetchedPaper = targetPaper
          this.$router.push(targetPath)
          return
        }
        
        this.paper = targetPaper

      } catch (error) {
        
        this.paper = canonifyPaper({
          kind: 'fault',
          data: {
            fault: 'NetworkError',
            reason: [ error.message ],
            stackTrace: error.stack,
          },
          self: this.$browser.href(this)
        })

      } finally {

        this.preFetchedPaper = null

      }
    },
  }
}
</script>
