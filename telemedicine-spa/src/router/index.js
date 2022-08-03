import { createRouter, createWebHistory } from 'vue-router'
// Default Component Page
import Dashboard from '@/views/Admin/Dashboard.vue'
import Login from '@/views/General/Login.vue'
import Signup from '@/views/General/Signup.vue'
import Home from '@/views/General/Home.vue'
import PageNotFound from '@/views/NotFound.vue'
import DoctorFeedback from '@/views/Patient/DoctorFeedback.vue'
import BookAppointment from '@/views/Patient/BookAppointment.vue'
import Patient from '@/views/Patient/Patient.vue'
import Payment from '@/views/Patient/Payment.vue'
import Chat from '@/views/Patient/Chat.vue'

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
    children: [
      { path: '/update-record', component: Patient }, 
      { path: '/book-service', component: BookAppointment },
      { path: '/payment', component: Payment },
      { path: '/chat', component: Chat },
      { path: '/feedback', component: DoctorFeedback },
    ],
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
  {
    path: '/feedback',
    name: 'DoctorFeedback',
    component: DoctorFeedback,
    meta: { title: "Doctor's feedback" },
  },
  {
    path: '/:catchAll(.*)',
    name: 'NotFound',
    component: PageNotFound,
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
