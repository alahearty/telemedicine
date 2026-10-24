import { createRouter, createWebHistory } from 'vue-router'
const Home = () => import(/* webpackChunkName: "public" */ '@/views/General/Home.vue')
const Login = () => import(/* webpackChunkName: "auth" */ '@/views/General/Login.vue')
const Signup = () => import(/* webpackChunkName: "auth" */ '@/views/General/Signup.vue')
const PageNotFound = () => import(/* webpackChunkName: "public" */ '@/views/NotFound.vue')
const DashboardLayout = () => import(/* webpackChunkName: "patient" */ '@/Layout/DashboardLayout.vue')
const PhysicianDashboardLayout = () => import(/* webpackChunkName: "physician" */ '@/Layout/PhysicianDashboardLayout.vue')
const Dashboard = () => import(/* webpackChunkName: "admin" */ '@/views/Admin/Dashboard.vue')
const HospitalManagement = () => import(/* webpackChunkName: "admin" */ '@/views/Admin/HospitalManagement.vue')
const Account = () => import(/* webpackChunkName: "admin" */ '@/views/Admin/Account.vue')
const DoctorFeedback = () => import(/* webpackChunkName: "patient" */ '@/views/Patient/DoctorFeedback.vue')
const BookAppointment = () => import(/* webpackChunkName: "patient-booking" */ '@/views/Patient/BookAppointment.vue')
const Patient = () => import(/* webpackChunkName: "patient" */ '@/views/Patient/Patient.vue')
const Payment = () => import(/* webpackChunkName: "patient-booking" */ '@/views/Patient/Payment.vue')
const Chat = () => import(/* webpackChunkName: "chat" */ '@/views/Patient/Chat.vue')
const DashboardPhysician = () => import(/* webpackChunkName: "physician" */ '@/views/Physician/DashboardPhysician.vue')
const AppointmentsPhysician = () => import(/* webpackChunkName: "physician" */ '@/views/Physician/AppointmentsPhysician.vue')
const PatientsPhysician = () => import(/* webpackChunkName: "physician" */ '@/views/Physician/PatientsPhysician.vue')
const ProfilePhysician = () => import(/* webpackChunkName: "physician" */ '@/views/Physician/ProfilePhysician.vue')
const UnattendedPhysician = () => import(/* webpackChunkName: "physician" */ '@/views/Physician/UnattendedPhysician.vue')
const AvailabilityPhysician = () => import(/* webpackChunkName: "physician" */ '@/views/Physician/AvailabilityPhysician.vue')
const DoctorDirectory = () => import(/* webpackChunkName: "patient" */ '@/views/Patient/DoctorDirectory.vue')
const DoctorProfile = () => import(/* webpackChunkName: "patient" */ '@/views/Patient/DoctorProfile.vue')
const AppointmentsPatient = () => import(/* webpackChunkName: "patient" */ '@/views/Patient/AppointmentsPatient.vue')
const AppointmentDetails = () => import(/* webpackChunkName: "appointments" */ '@/views/Appointment/AppointmentDetails.vue')
const Consultation = () => import('@/views/Consultation/Consultation.vue')

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
      { path: 'doctors', name: 'DoctorDirectory', component: DoctorDirectory },
      { path: 'doctors/:id', name: 'DoctorProfile', component: DoctorProfile },
      { path: 'appointments', name: 'PatientAppointments', component: AppointmentsPatient },
      { path: 'appointments/:id', name: 'PatientAppointmentDetails', component: AppointmentDetails },
      { path: 'consultations/:sessionId', name: 'PatientConsultation', component: Consultation },
      { path: 'chat', name: 'PatientChat', component: Chat },
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
      { path: 'appointments/:id', name: 'PhysicianAppointmentDetails', component: AppointmentDetails },
      { path: 'consultations/:sessionId', name: 'PhysicianConsultation', component: Consultation },
      { path: 'chat', name: 'PhysicianChat', component: Chat },
      { path: 'availability', name: 'PhysicianAvailability', component: AvailabilityPhysician },
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
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,

  linkExactActiveClass: 'exact-active',
  scrollBehavior(to, from, savedPosition) {
    return savedPosition || { top: 0 }
  },
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
