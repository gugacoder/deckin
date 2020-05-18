import Vue from 'vue'
import VueRouter from 'vue-router'
import Desktop from '@/views/Desktop.vue'
import Login from '@/views/Login.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    redirect: '/Desktop'
  },
  {
    path: '/Desktop',
    name: 'Desktop',
    component: Desktop
  },
  {
    path: '/Login',
    name: 'Login',
    component: Login
  },
  {
    path: '/About',
    name: 'About',
    component: () => import(/* webpackChunkName: "about" */ '@/views/About.vue')
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

// Interceptação de navegação para acionamento do Login
//
/*
router.beforeEach((to, from, next) => {
  const publicPages = [ '/login' ];
  const isAuthRequired = !publicPages.includes(to.path);
  const isLoggedIn = localStorage.getItem('user');

  if (isAuthRequired && !isLoggedIn) {
    return next({
      path: '/login',
      query: {
        forward: to.fullPath
      }
    });
  }

  next();
})
*/

export default router
