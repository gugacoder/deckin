<template>
  <v-app class="desktop">
    <the-header
      :title="$options.name"
    >
    </the-header>

    <the-content>
      <cards-paper
        :entity="catalog"
      >
      </cards-paper>
    </the-content>
  </v-app>
</template>

<script>
import TheHeader from '@/components/layout/TheHeader.vue'
import TheContent from '@/components/layout/TheContent.vue'
import CardsPaper from '@/components/papers/CardsPaper.vue'

export default {
  name: 'Desktop',

  components: {
    TheHeader,
    TheContent,
    CardsPaper,
  },

  data: () => {
  },

  computed: {
    catalog: function() {
      return {
        class: [ 'records' ],
        metas: {
          paper: [ 'cards' ],
          headers: {
            record: {
              id: {
                visible: true,
                title: '#',
                type: 'number',
                sortOrder: 1
              },
              name: {
                visible: true,
                title: 'Name',
                type: 'text',
                sortOrder: 0
              },
              description: {
                visible: true,
                title: 'Description',
                type: 'text',
                description: 0
              }
            }
          }
        },
        entities: [
          {
            rel: [ 'record' ],
            props: {
              id: 1,
              name: 'Grid Sample',
              description: 'Sample page demonstrating a GridPaper.',
            },
            metas: {
              headers: {
                record: '@/metas/headers/record'
              }
            },
          },
          {
            rel: [ 'record' ],
            props: {
              id: 2,
              name: 'Form Sample',
              description: 'Sample page demonstrating a FormPaper.',
            },
            metas: {
              headers: {
                record: '@/metas/headers/record'
              }
            },
          }
        ],
        links: [
          {
            rel: [ 'self '],
            href: '/Api/1/Catalog'
          }
        ]
      }
    }
  }
}
</script>
