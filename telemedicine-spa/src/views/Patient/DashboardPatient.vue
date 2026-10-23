<template>
  <div class="patient-dashboard p-6">
    <!-- Process tracker breadcrumb -->
    <nav class="flex mb-8" aria-label="Breadcrumb">
      <ol class="inline-flex items-center flex-wrap gap-2 text-sm">
        <li class="inline-flex items-center">
          <router-link
            to="/dashboard"
            class="text-slate-600 dark:text-slate-400 hover:text-primary-600 dark:hover:text-primary-400 font-medium"
          >
            Home
          </router-link>
        </li>
        <li v-for="(item, i) in processSteps" :key="item.url" class="inline-flex items-center">
          <span class="mx-2 text-slate-300 dark:text-slate-600">›</span>
          <router-link
            :to="item.url"
            class="text-slate-600 dark:text-slate-400 hover:text-primary-600 dark:hover:text-primary-400 font-medium"
          >
            {{ item.label }}
          </router-link>
        </li>
      </ol>
    </nav>

    <!-- Welcome & Quick stats -->
    <div class="mb-8">
      <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-1">
        Welcome back{{ userDisplayName ? `, ${userDisplayName}` : '' }}
      </h1>
      <p class="text-slate-600 dark:text-slate-400">
        Manage your healthcare appointments and connect with specialists 24/7
      </p>
    </div>

    <!-- Quick action cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
      <router-link
        to="/dashboard/book-service"
        class="card-modern p-6 flex items-center gap-4 hover:shadow-lg transition-shadow border-2 border-transparent hover:border-primary-200 dark:hover:border-primary-800 group"
      >
        <div class="w-14 h-14 rounded-2xl bg-primary-100 dark:bg-primary-900/40 flex items-center justify-center text-primary-600 dark:text-primary-400 group-hover:scale-105 transition-transform">
          <Icon icon="bxs:calendar-plus" class="text-2xl" />
        </div>
        <div>
          <h3 class="font-semibold text-slate-800 dark:text-slate-100">Book Appointment</h3>
          <p class="text-sm text-slate-500 dark:text-slate-400">Schedule a consultation</p>
        </div>
      </router-link>

      <router-link
        to="/dashboard/update-record"
        class="card-modern p-6 flex items-center gap-4 hover:shadow-lg transition-shadow border-2 border-transparent hover:border-primary-200 dark:hover:border-primary-800 group"
      >
        <div class="w-14 h-14 rounded-2xl bg-emerald-100 dark:bg-emerald-900/40 flex items-center justify-center text-emerald-600 dark:text-emerald-400 group-hover:scale-105 transition-transform">
          <Icon icon="bxs:user-detail" class="text-2xl" />
        </div>
        <div>
          <h3 class="font-semibold text-slate-800 dark:text-slate-100">Update Record</h3>
          <p class="text-sm text-slate-500 dark:text-slate-400">Your medical profile</p>
        </div>
      </router-link>

      <router-link
        to="/dashboard/doctors"
        class="card-modern p-6 flex items-center gap-4 hover:shadow-lg transition-shadow border-2 border-transparent hover:border-primary-200 dark:hover:border-primary-800 group"
      >
        <div class="w-14 h-14 rounded-2xl bg-violet-100 dark:bg-violet-900/40 flex items-center justify-center text-violet-600 dark:text-violet-400 group-hover:scale-105 transition-transform">
          <Icon icon="bxs:user-account" class="text-2xl" />
        </div>
        <div>
          <h3 class="font-semibold text-slate-800 dark:text-slate-100">Find a Doctor</h3>
          <p class="text-sm text-slate-500 dark:text-slate-400">Browse doctors and availability</p>
        </div>
      </router-link>

      <router-link
        to="/dashboard/appointments"
        class="card-modern p-6 flex items-center gap-4 hover:shadow-lg transition-shadow border-2 border-transparent hover:border-primary-200 dark:hover:border-primary-800 group"
      >
        <div class="w-14 h-14 rounded-2xl bg-amber-100 dark:bg-amber-900/40 flex items-center justify-center text-amber-600 dark:text-amber-400 group-hover:scale-105 transition-transform">
          <Icon icon="bxs:calendar-check" class="text-2xl" />
        </div>
        <div>
          <h3 class="font-semibold text-slate-800 dark:text-slate-100">My Appointments</h3>
          <p class="text-sm text-slate-500 dark:text-slate-400">History, reschedule and cancel</p>
        </div>
      </router-link>
    </div>

    <!-- Upcoming appointments & Recent activity -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="card-modern p-6">
        <h2 class="font-display text-lg font-semibold text-slate-800 dark:text-slate-100 mb-4">
          Upcoming Appointments
        </h2>
        <div v-if="upcomingAppointments.length" class="space-y-4">
          <div
            v-for="apt in upcomingAppointments"
            :key="apt.id"
            class="flex items-center justify-between p-4 rounded-xl bg-slate-50 dark:bg-slate-800/50 border border-slate-100 dark:border-slate-700"
          >
            <div>
              <p class="font-medium text-slate-800 dark:text-slate-100">{{ apt.serviceName || 'Consultation' }}</p>
              <p class="text-sm text-slate-500 dark:text-slate-400">{{ apt.date }} at {{ apt.time }}</p>
            </div>
            <div class="flex items-center gap-3">
              <span class="px-3 py-1 rounded-full text-xs font-medium" :class="statusClass(apt.rawStatus)">{{ apt.status }}</span>
              <router-link :to="`/dashboard/appointments/${apt.id}`" class="text-xs text-primary-600 hover:text-primary-700 font-medium">Details</router-link>
              <button class="text-xs text-red-600 hover:text-red-700 font-medium" @click="cancel(apt)">Cancel</button>
            </div>
          </div>
        </div>
        <div v-else class="text-center py-12 text-slate-500 dark:text-slate-400">
          <Icon icon="bxs:calendar-x" class="text-4xl mb-4 opacity-50" />
          <p>No upcoming appointments</p>
          <router-link to="/dashboard/book-service" class="text-primary-600 dark:text-primary-400 font-medium mt-2 inline-block hover:underline">
            Book your first appointment →
          </router-link>
        </div>
      </div>

      <div class="card-modern p-6">
        <h2 class="font-display text-lg font-semibold text-slate-800 dark:text-slate-100 mb-4">
          Quick Links
        </h2>
        <div class="space-y-3">
          <router-link
            to="/dashboard/appointments"
            class="flex items-center gap-3 p-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
          >
            <Icon icon="bxs:calendar-check" class="text-xl text-primary-500" />
            <span class="font-medium text-slate-700 dark:text-slate-300">Manage appointments</span>
          </router-link>
          <router-link
            to="/dashboard/doctors"
            class="flex items-center gap-3 p-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
          >
            <Icon icon="bxs:user-account" class="text-xl text-primary-500" />
            <span class="font-medium text-slate-700 dark:text-slate-300">Find doctors</span>
          </router-link>
          <router-link
            to="/dashboard/account"
            class="flex items-center gap-3 p-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
          >
            <Icon icon="bxs:user-circle" class="text-xl text-primary-500" />
            <span class="font-medium text-slate-700 dark:text-slate-300">Account Settings</span>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { Icon } from '@iconify/vue'
import { getPatientAppointments, getCurrentPatient, cancelAppointment } from '@/api'
import { isUpcoming, statusClass, statusLabel as labelForStatus } from '@/utils/appointments'

export default {
  name: 'DashboardPatient',
  components: { Icon },
  data() {
    return {
      processSteps: [
        { label: 'Find a Doctor', url: '/dashboard/doctors' },
        { label: 'Book Appointment', url: '/dashboard/book-service' },
        { label: 'My Appointments', url: '/dashboard/appointments' },
        { label: 'Update Record', url: '/dashboard/update-record' },
      ],
      upcomingAppointments: [],
      userProfile: null,
    }
  },
  computed: {
    userDisplayName() {
      const name = localStorage.getItem('user-name') || this.userProfile?.firstName
      if (name) {
        const last = this.userProfile?.lastName
        return last ? `${name} ${last}` : name
      }
      const email = localStorage.getItem('user-email')
      return email ? email.split('@')[0] : null
    },
  },
  methods: {
    statusLabel(status) {
      return labelForStatus(status)
    },
    statusClass,
    async cancel(appointment) {
      if (!window.confirm('Cancel this appointment?')) return
      try {
        await cancelAppointment(appointment.id)
        if (this.$notification) this.$notification.success('Cancelled', 'The appointment was cancelled.')
        await this.loadAppointments()
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message || 'Could not cancel the appointment.')
      }
    },
    async loadProfile() {
      try {
        const p = await getCurrentPatient()
        this.userProfile = p
        if (p?.firstName) localStorage.setItem('user-name', `${p.firstName} ${p.lastName || ''}`.trim())
        if (p?.id) localStorage.setItem('user-id', String(p.id))
      } catch (e) {
        console.warn('Could not load patient profile:', e)
      }
    },
    async loadAppointments() {
      try {
        const patientId = localStorage.getItem('user-id') || this.userProfile?.id
        if (patientId) {
          const data = await getPatientAppointments(patientId)
          const list = Array.isArray(data) ? data : (data?.data ?? [])
          this.upcomingAppointments = list
            .filter(a => isUpcoming(a.status))
            .slice(0, 5)
            .map(a => {
              const when = a.appointmentDate || a.scheduleStartTime
              return {
                id: a.id,
                serviceName: a.serviceName || a.telemedicineService?.serviceName,
                date: when ? new Date(when).toLocaleDateString() : 'TBD',
                time: when ? new Date(when).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' }) : 'TBD',
                status: this.statusLabel(a.status),
                rawStatus: a.status,
              }
            })
        }
      } catch (e) {
        console.warn('Could not load appointments:', e)
      }
    },
  },
  mounted() {
    this.loadProfile().then(() => this.loadAppointments())
  },
}
</script>

<style scoped>
.patient-dashboard {
  max-width: 1200px;
  margin: 0 auto;
}
</style>
