import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '@/views/Admin/Dashboard.vue'
import DashboardLayout from '@/Layout/DashboardLayout.vue'
import PhysicianDashboardLayout from '@/Layout/PhysicianDashboardLayout.vue'
import Login from '@/views/General/Login.vue'
import Signup from '@/views/General/Signup.vue'
import Home from '@/views/General/Home.vue'
import PageNotFound from '@/views/NotFound.vue'
import HospitalManagement from '@/views/Admin/HospitalManagement.vue'
import Account from '@/views/Admin/Account.vue'
import DoctorFeedback from '@/views/Patient/DoctorFeedback.vue'
import BookAppointment from '@/views/Patient/BookAppointment.vue'
import Patient from '@/views/Patient/Patient.vue'
import Payment from '@/views/Patient/Payment.vue'
import Chat from '@/views/Patient/Chat.vue'
import DashboardPhysician from '@/views/Physician/DashboardPhysician.vue'
import AppointmentsPhysician from '@/views/Physician/AppointmentsPhysician.vue'
import PatientsPhysician from '@/views/Physician/PatientsPhysician.vue'
import ProfilePhysician from '@/views/Physician/ProfilePhysician.vue'
import UnattendedPhysician from '@/views/Physician/UnattendedPhysician.vue'

var appname = ' - Analytics Dashboard'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: { title: 'Telemedicine' },
  },
  {
    path: '/dashboard',
    component: DashboardLayout,
    meta: { title: 'Dashboard ' + appname },
    children: [
      { path: '', name: 'Dashboard', component: Dashboard },
      { path: 'account', component: Account },
      { path: 'update-record', component: Patient }, 
      { path: 'book-service', component: BookAppointment },
      { path: 'payment', component: Payment },
      { path: 'chat', component: Chat },
      { path: 'feedback', component: DoctorFeedback },
      { path: 'hospital', name: 'HospitalManagement', component: HospitalManagement },
    ],
  },
  {
    path: '/physician',
    component: PhysicianDashboardLayout,
    meta: { title: 'Physician Dashboard ' + appname, role: 'physician' },
    children: [
      { path: '', name: 'PhysicianDashboard', component: DashboardPhysician },
      { path: 'appointments', name: 'PhysicianAppointments', component: AppointmentsPhysician },
      { path: 'patients', name: 'PhysicianPatients', component: PatientsPhysician },
      { path: 'profile', name: 'PhysicianProfile', component: ProfilePhysician },
      { path: 'unattended', name: 'PhysicianUnattended', component: UnattendedPhysician },
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
  const publicRoutes = ['/', '/login', '/signup']
  const isPublicRoute = publicRoutes.includes(to.path)
  
  // Get authentication token
  const token = localStorage.getItem('access-token')
  const isAuthenticated = !!token
  
  // Check if route requires authentication
  const requiresAuth = !isPublicRoute && to.path !== '/'
  
  if (requiresAuth && !isAuthenticated) {
    next({ path: '/login', query: { redirect: to.fullPath } })
    return
  }
  if (isAuthenticated) {
    const role = (localStorage.getItem('user-role') || '').toLowerCase()
    const isPhysician = role.includes('physician') || role.includes('doctor')
    const isAdmin = role.includes('admin') || role.includes('administrator')
    // Physician trying to access dashboard -> redirect to physician
    if (isPhysician && to.path.startsWith('/dashboard')) {
      next({ path: '/physician' })
      return
    }
    // Admin/Patient trying to access physician area -> redirect to dashboard
    if (!isPhysician && to.path.startsWith('/physician')) {
      next({ path: '/dashboard' })
      return
    }
  }
  if (isAuthenticated && (to.path === '/login' || to.path === '/signup')) {
    const role = (localStorage.getItem('user-role') || '').toLowerCase()
    const dest = (role.includes('physician') || role.includes('doctor')) ? '/physician' : '/dashboard'
    next({ path: dest })
    return
  }
  next()
})

export default router
