import Vue from 'vue'
import VueRouter from 'vue-router'
//import HomeView from '@/views/HomeView.vue'
import PaperView from '@/views/PaperView.vue'
import ActionView from '@/views/ActionView.vue'
import NotFoundView from '@/views/NotFoundView.vue'
import AppConfig from '@/AppConfig.js'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Index',
    //component: HomeView,
    //meta: {
    //  title: AppConfig.title
    //}
    //redirect: { path: '/!/App/Home/Index' }
    //redirect: { path: '/Sandbox/Tananana' },
    //redirect: { path: '/!/Keep.Paper/Home/Index' }
    redirect: { path: '/!/App/Home' }
  },
  {
    path: '/!/:workspace/:action',
    name: 'Action',
    component: ActionView,
    props: true,
    meta: {
      title: AppConfig.title
    }
  },


  {
    // XXX: OBSOLETE
    path: '/Home',
    name: 'Home',
    redirect: { path: '/!/Keep.Paper/Home/Index' }
  },
  {
    // XXX: OBSOLETE
    // TODO: new route pattern
    //  path: '/!/:workspace/:action/:keys?',
    //  workspace and action are the same pattern: Foo.Bar
    path: '/!/:catalogName/:paperName/:actionName/:actionKeys?',
    name: 'Paper',
    component: PaperView,
    props: true,
    meta: {
      title: AppConfig.title
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
  document.title = (to.meta && to.meta.title) || AppConfig.title
  next();
});

export default router
