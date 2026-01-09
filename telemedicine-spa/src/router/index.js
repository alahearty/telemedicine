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

// Route guard for authentication
router.beforeEach((to, from, next) => {
  document.title = to.meta.title || 'Telemedicine'
  
  // Public routes that don't require authentication
  const publicRoutes = ['/', '/login', '/signup', '/Home']
  const isPublicRoute = publicRoutes.includes(to.path)
  
  // Get authentication token
  const token = localStorage.getItem('access-token')
  const isAuthenticated = !!token
  
  // Check if route requires authentication
  const requiresAuth = !isPublicRoute && to.path !== '/'
  
  if (requiresAuth && !isAuthenticated) {
    // Redirect to login if trying to access protected route without authentication
    next({ path: '/login', query: { redirect: to.fullPath } })
  } else if (isAuthenticated && (to.path === '/login' || to.path === '/signup')) {
    // Redirect authenticated users away from login/signup pages
    next({ path: '/dashboard' })
  } else {
    next()
  }
})

export default router
