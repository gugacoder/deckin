import Vue from 'vue'
import VueRouter from 'vue-router'
import PaperView from '../views/PaperView.vue'
import NotFoundView from '../views/NotFoundView.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    redirect: { path: '/!/App/Home/Index' }
  },
  {
    path: '/!/:catalogName/:paperName/:actionName/:actionKeys?',
    name: 'Paper',
    component: PaperView,
    props: true,
    meta: {
      title: 'Mercadologic™'
    }
  },
  {
    path: '/Sandbox/:arg',
    name: 'Sandbox',
    // route level code-splitting
    // this generates a separate chunk (sandbox.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import(/* webpackChunkName: "sandbox" */ '../views/Sandbox.vue')
  },
  {
    path: '*',
    name: 'NotFound',
    component: NotFoundView
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

router.beforeEach((to, from, next) => {
  document.title = (to.meta && to.meta.title) || 'Paper App'
  next();
});

export default router
