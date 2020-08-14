<template lang="pug">
  v-app-bar.the-paper-header(
    app
    color="primary"
    dark
    absolute
    clipped-right
    clipped-left
    :class="showCaption ? 'pt-4' : undefined"
    :height="showCaption ? 90 : undefined"
    :prominent="prominent"
    :shrink-on-scroll="prominent"
    :fade-img-on-scroll="prominent"
    :src="prominent ? '/img/header.jpg' : ''"
    scroll-target="#x-content"
    scroll-threshold="200"
  )
    div#x-caption-bar(
      v-if="showCaption"
    )
      template(
        v-if="!$isMobile"
      )
        app-title.mr-2

        span.primary--text.text--lighten-2.mr-1(
          v-if="catalog"
        )
          span.text-no-wrap / {{ catalog }} /

      span.font-weight-medium.text-no-wrap {{ caption }}

    template(
      v-slot:img="{ props }"
    )
      v-img(
        v-bind="props"
        :gradient="`to top right, var(--v-primary-${dark ? 'darken2' : 'base'}), rgba(65, 71, 81, 0)`"
      )

    slot(
      name="first"
    )

    v-toolbar-title.pl-8(
      v-if="!noTitle"
    )
      template(
        v-if="title"
      )
        | {{ title }}

      template(
        v-else
      )
        app-title

    slot(
      name="left"
    )

    v-spacer

    slot
          
    slot(
      name="right"
    )
</template>

<style>
#x-caption-bar {
  display: inline-flex;
  position: fixed;
  left: 0;
  top: 0;
  padding: 8px 18px;
  width: 100%;
  height: 34px;
}

.the-paper-header .v-toolbar__content {
  padding-left: 4px;
}
</style>

<script>
//import Vue from 'vue'
import { mapState } from 'vuex'
import AppTitle from '@/components/layout/AppTitle.vue'

export default {
  name: 'the-header',

  components: {
    AppTitle,
  },

  props: {
    title: {
      type: String,
    },
    
    catalog: {
      type: String,
    },
    
    caption: {
      type: String,
    },

    prominent: {
      type: Boolean,
    },
    
    noTitle: {
      type: Boolean,
    },
  },

  data: () => ({
  }),

  computed: {
    ...mapState('system', [
      'dark',
    ]),

    showCaption () {
      return this.caption && !this.prominent
    },
  },

  methods: {
  }
}
</script>