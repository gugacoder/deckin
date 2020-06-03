<template>
  <div class="data-paper">
    <v-card
      class="mx-auto"
    >
      <v-card-title>
        {{ title }}
      </v-card-title>

      <v-card-text>
        <div
          v-for="field in fields"
          :key="field.name"
        >
        <p>
          <span
            v-show="field.kind !== 'information'"
          >
            {{ field.name }}
            
            <br>
          </span>

          <span
            v-if="field.linkTo"
          >
            <router-link
              :to="makeLink(field.linkTo)"
            >
            {{ fieldValue(field.name) }}
            </router-link>
          </span>

          <span
            v-else
            class="text--primary"
          >
            {{ fieldValue(field.name) }}
          </span>
        </p>
<!--
          <div
            v-if="field.kind === 'information'"
            class="text--primary"
          >
            <p v-if="field.linkTo">
              <router-link
                :to="field.linkTo"
              >
                {{ fieldValue(field.name) }}
              </router-link>
            </p>
            <p v-else>
              {{ fieldValue(field.name) }}
            </p>
          </div>
          <div
            v-else
          >

          </div>
        -->
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import lodash from 'lodash'

export default {
  name: 'data-paper',

  props: [
    'catalogName',
    'paperName',
    'paperAction',
    'paperKeys',
    'paper'
  ],

  computed: {
    title () {
      if (this.paper.title) return this.paper.title;

      var tokens = [
        this.paperName, 
        this.paperAction,
        ...this.paperKeys
      ];

      return tokens.join(' / ');
    },

    fields () {
      lodash.startCase('');
      if (!this.paper) return [];

      if (this.paper.fields) {
        return Object.keys(this.paper.fields).map(name => ({
          name,
          ...this.paper.fields[name]
        }));
      }

      /*
      if (!this.paper.data) return [];

      return this.paper.data.keys.map(name => ({
        name,
        title: lodash.startCase(name),
        hidden: name.startsWith('_'),
        value () { this.paper.data[name] }
      }));
      */

      return [];
    }
  },

  methods: {
    fieldValue (fieldName) {
      return this.paper.data[fieldName];
    },

    makeLink (rel) {
      var link = this.paper.links.filter(link => link.rel === rel)[0];
      if (!link) return '';

      var href = link.href;
      if (!href) return '';
 
      var path = href.split('!/')[1];
      if (!path) return '';

      return `/Papers/${path}`;
    }
  }
}
</script>