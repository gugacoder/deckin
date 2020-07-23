<template lang="pug">
  //-
    v-text-field.date-widget(
      v-show="!hidden"
      v-model="value"
      :label="title"
      :hint="hint"
      :class="`extent-${extent}`"
      disabled
      dense
    )
  v-menu(
    v-model="menu"
    :close-on-content-click="false"
    :nudge-right="40"
    transition="scale-transition"
    offset-y
    min-width="290px"
  )
    template(
      v-slot:activator="{ on, attrs }"
    )
      v-text-field(
        v-show="!hidden"
        v-model="text"
        :label="title"
        :rules="rules"
        :required="required"
        :hint="hint"
        :class="`extent-${extent}`"
        prepend-icon="mdi-calendar"
        readonly
        dense
        v-bind="attrs"
        v-on="on"
      )

    v-date-picker(
      v-model="date"
      no-title
      @input="menu=false"
    )
</template>

<script>
import WidgetBase from './-WidgetBase.vue'
import moment from 'moment'

export default {
  extends: WidgetBase,
  
  name: 'date-widget',

  data: () => ({
    defaultExtent: 1,
    menu: null,
  }),

  computed: {
    date: {
      get () {
        return moment(this.value).format('YYYY-MM-DD')
      },
      set (value) {
        this.value = moment(value).toDate()
      }
    },

    text () {
      return this.value ? moment(this.value).format('DD/MM/YYYY') : null
    }
  }
}
</script>