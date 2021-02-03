<template lang="pug">
  v-navigation-drawer.app-drawer(
    app
    style="z-index:3;"
    :right="right"
    :width="extent"
    :value="value"
    @input="value => $emit('input', value)"
  )
    v-toolbar.x-header(
      color="primary"
      dark
      style="cursor:pointer;"
      @click.stop="$emit('input', false)"
    )
      v-btn(
        v-if="!right && !$isMobile"
        icon
        @click.stop="$emit('input', false)"
      )
        v-icon {{ icon || 'mdi-close' }}

      v-btn(
        v-if="right && !$isMobile"
        icon
        @click.stop="extended = !extended"
      )
        v-icon {{ extended ? 'mdi-chevron-right' : 'mdi-chevron-left' }}
      
      v-btn(
        v-show="title"
        text
        large
        style=""
        @click.stop="$emit('input', false)"
      )
        | {{ title }}

      v-spacer

      v-btn(
        v-if="right || $isMobile"
        icon
        @click.stop="$emit('input', false)"
      )
        v-icon {{ icon || 'mdi-close' }}

      v-btn(
        v-if="!right && !$isMobile"
        icon
        @click.stop="extended = !extended"
      )
        v-icon {{ extended ? 'mdi-chevron-left' : 'mdi-chevron-right' }}

    div.x-content(
      :class="noFooter ? 'x-no-footer' : undefined"
    )
      slot

    v-toolbar.x-footer(
      v-if="!noFooter"
    )
      slot(
        name="left"
      )

      v-spacer(
        v-if="!right"
      )

      slot(
        name="footer"
      )

      v-spacer(
        v-if="right"
      )

      slot(
        name="right"
      )
</template>

<style scoped>
.x-header {
  z-index: 1;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  overflow: hidden;
}

.x-content {
  position: fixed;
  top: 56px;
  left: 0;
  right: 0;
  padding-top: 10px;
  padding-bottom: 10px;
  overflow-x: hidden;
  overflow-y: scroll;
}

.x-content.x-no-footer {
  bottom: 0px;
}

.x-content:not(.x-no-footer) {
  bottom: 56px;
}

.x-footer {
  position: fixed;
  left: 0;
  right: 0;
  bottom: 0;
  overflow: hidden;
  text-align: right;
  background-color: rgba(0,0,0,.05);
}
</style>

<script>
export default {
  name: 'app-drawer',

  props: {
    value: {
      type: Boolean
    },

    title: {
      type: String
    },

    right: {
      type: Boolean
    },

    extendable: {
      type: Boolean
    },

    icon: {
      type: String
    },

    noFooter: {
      type: Boolean
    }
  },

  data: () => ({
    extended: false,
    minified: false,
  }),

  computed: {
    extent () {
      if (this.$isMobile)
        return '100%'
      
      return this.extended ? 600 : 320
    },
  }
}
</script>
