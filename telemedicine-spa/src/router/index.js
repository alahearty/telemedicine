import { createRouter, createWebHistory } from 'vue-router'
// Default Component Page
import Dashboard from '../views/Admin/Dashboard.vue'
import Login from '../views/General/Login.vue'
import Signup from '../views/General/Signup.vue'
import Home from '../views/General/Home.vue'
var appname = ' - Analytics Dashboard'

const routes = [
  // Routes
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: { title: 'Telemedicine' },
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: Dashboard,
    meta: { title: 'Dashboard ' + appname },
  },
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { title: 'Login' },
  },
  {
    path: '/signup',
    name: 'Signup',
    component: Signup,
    meta: { title: 'Signup' },
  },
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,

  linkExactActiveClass: 'exact-active',
})

router.beforeEach((to, from, next) => {
  document.title = to.meta.title
  next()
})

export default router
