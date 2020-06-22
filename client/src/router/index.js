import Vue from 'vue'
import VueRouter from 'vue-router'
import Paper from '../views/Paper.vue'
import NotFound from '../views/NotFound.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    redirect: { path: '/Papers/Keep.Paper/Home/Index' }
//    redirect: { path: '/Sandbox' }
  },
  {
    path: '/Papers/*',
    name: 'Paper',
    component: Paper
    /*
    meta: {
      reload: true,
    },
    */
  },
  {
    path: '/sandbox',
    name: 'Sandbox',
    // route level code-splitting
    // this generates a separate chunk (sandbox.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "sandbox" */ '../views/Sandbox.vue')
  },
  {
    path: '*',
    name: 'NotFound',
    component: NotFound
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
