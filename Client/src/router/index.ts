import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Paper from '@/views/Paper.vue'
import NotFound from '@/views/NotFound.vue'
import Splash from '@/views/Splash.vue'

Vue.use(VueRouter)

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'Index',
    redirect: '/!'
  },
  {
    path: '/!',
    name: 'Splash',
    component: Splash,
  },
  {
    path: '/!/:paper',
    name: 'Paper',
    component: Paper,
    props: true
  },
  {
    path: '*',
    name: 'NotFound',
    component: NotFound,
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
