<template>
  <div class="physician-dashboard p-6">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-1">
      Welcome back{{ displayName ? `, ${displayName}` : '' }}
    </h1>
    <p class="text-slate-600 dark:text-slate-400 mb-8">
      Manage your consultations and patient care
    </p>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
      <div class="card-modern p-6">
        <div class="flex items-center gap-4">
          <div class="w-14 h-14 rounded-2xl bg-primary-100 dark:bg-primary-900/40 flex items-center justify-center text-primary-600">
            <Icon icon="bxs:calendar" class="text-2xl" />
          </div>
          <div>
            <p class="text-2xl font-bold text-slate-800 dark:text-slate-100">{{ stats.totalAppointments }}</p>
            <p class="text-sm text-slate-500">Total Appointments</p>
          </div>
        </div>
      </div>
      <div class="card-modern p-6">
        <div class="flex items-center gap-4">
          <div class="w-14 h-14 rounded-2xl bg-amber-100 dark:bg-amber-900/40 flex items-center justify-center text-amber-600">
            <Icon icon="bxs:bell" class="text-2xl" />
          </div>
          <div>
            <p class="text-2xl font-bold text-slate-800 dark:text-slate-100">{{ stats.unattended }}</p>
            <p class="text-sm text-slate-500">Unattended</p>
          </div>
        </div>
      </div>
      <div class="card-modern p-6">
        <div class="flex items-center gap-4">
          <div class="w-14 h-14 rounded-2xl bg-emerald-100 dark:bg-emerald-900/40 flex items-center justify-center text-emerald-600">
            <Icon icon="bxs:user-check" class="text-2xl" />
          </div>
          <div>
            <p class="text-2xl font-bold text-slate-800 dark:text-slate-100">{{ stats.completed }}</p>
            <p class="text-sm text-slate-500">Completed</p>
          </div>
        </div>
      </div>
      <div class="card-modern p-6">
        <div class="flex items-center gap-4">
          <div class="w-14 h-14 rounded-2xl bg-violet-100 dark:bg-violet-900/40 flex items-center justify-center text-violet-600">
            <Icon icon="bxs:calendar-check" class="text-2xl" />
          </div>
          <div>
            <p class="text-2xl font-bold text-slate-800 dark:text-slate-100">{{ stats.active }}</p>
            <p class="text-sm text-slate-500">Active Today</p>
          </div>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="card-modern p-6">
        <h2 class="font-display text-lg font-semibold text-slate-800 dark:text-slate-100 mb-4">
          Upcoming Appointments
        </h2>
        <div v-if="upcomingAppointments.length" class="space-y-4">
          <div
            v-for="apt in upcomingAppointments"
            :key="apt.id"
            class="flex items-center justify-between p-4 rounded-xl bg-slate-50 dark:bg-slate-800/50"
          >
            <div>
              <p class="font-medium text-slate-800 dark:text-slate-100">{{ apt.serviceName || 'Consultation' }}</p>
              <p class="text-sm text-slate-500">{{ apt.date }} at {{ apt.time }}</p>
            </div>
            <span class="px-3 py-1 rounded-full text-xs font-medium bg-primary-100 text-primary-600">
              {{ apt.status }}
            </span>
          </div>
        </div>
        <div v-else class="text-center py-8 text-slate-500">
          <Icon icon="bxs:calendar-x" class="text-4xl mb-2 opacity-50" />
          <p>No upcoming appointments</p>
        </div>
      </div>

      <div class="card-modern p-6">
        <h2 class="font-display text-lg font-semibold text-slate-800 dark:text-slate-100 mb-4">
          Quick Actions
        </h2>
        <div class="space-y-3">
          <router-link
            to="/physician/appointments"
            class="flex items-center gap-3 p-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
          >
            <Icon icon="bxs:calendar" class="text-xl text-primary-500" />
            <span class="font-medium text-slate-700 dark:text-slate-300">View All Appointments</span>
          </router-link>
          <router-link
            to="/physician/unattended"
            class="flex items-center gap-3 p-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
          >
            <Icon icon="bxs:bell" class="text-xl text-amber-500" />
            <span class="font-medium text-slate-700 dark:text-slate-300">Unattended Requests</span>
          </router-link>
          <router-link
            to="/physician/patients"
            class="flex items-center gap-3 p-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
          >
            <Icon icon="bxs:user-detail" class="text-xl text-primary-500" />
            <span class="font-medium text-slate-700 dark:text-slate-300">Patient List</span>
          </router-link>
          <router-link
            to="/physician/profile"
            class="flex items-center gap-3 p-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
          >
            <Icon icon="bxs:user-circle" class="text-xl text-primary-500" />
            <span class="font-medium text-slate-700 dark:text-slate-300">Profile Settings</span>
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { Icon } from '@iconify/vue'
import { getCurrentPhysician, getPhysicianAppointments, getUnattendedAppointments } from '@/api'

export default {
  name: 'DashboardPhysician',
  components: { Icon },
  data() {
    return {
      profile: null,
      appointments: [],
      unattended: [],
    }
  },
  computed: {
    displayName() {
      if (this.profile?.firstName) return `Dr. ${this.profile.firstName}`
      return this.profile?.lastName ? `Dr. ${this.profile.lastName}` : null
    },
    stats() {
      const completed = this.appointments.filter(a => this.statusLabel(a.status) === 'Attended').length
      const active = this.appointments.filter(a => this.statusLabel(a.status) === 'Active').length
      return {
        totalAppointments: this.appointments.length,
        unattended: this.unattended.length,
        completed,
        active,
      }
    },
    upcomingAppointments() {
      return this.appointments
        .filter(a => {
          const status = this.statusLabel(a.status)
          return status !== 'Cancelled' && status !== 'Attended'
        })
        .slice(0, 5)
        .map(a => {
          const when = a.appointmentDate || a.scheduleStartTime
          return {
            id: a.id,
            serviceName: a.serviceName || a.telemedicineService?.serviceName,
            date: when ? new Date(when).toLocaleDateString() : 'N/A',
            time: when ? new Date(when).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' }) : 'TBD',
            status: this.statusLabel(a.status),
          }
        })
    },
  },
  methods: {
    statusLabel(status) {
      if (typeof status === 'string') return status
      const labels = { 0: 'Active', 1: 'Not attended', 2: 'Attended', 3: 'Cancelled' }
      return labels[status] || 'Unknown'
    },
    async load() {
      try {
        const p = await getCurrentPhysician()
        this.profile = p
        if (p?.id) localStorage.setItem('physician-id', String(p.id))
      } catch (e) {
        console.warn('Could not load physician:', e)
      }
      try {
        const id = localStorage.getItem('physician-id') || this.profile?.id
        if (id) {
          const data = await getPhysicianAppointments(id)
          this.appointments = Array.isArray(data) ? data : (data?.data ?? [])
        }
      } catch (e) {
        console.warn('Could not load appointments:', e)
      }
      try {
        const data = await getUnattendedAppointments()
        this.unattended = Array.isArray(data) ? data : (data?.data ?? [])
      } catch (e) {
        console.warn('Could not load unattended:', e)
      }
    },
  },
  mounted() {
    this.load()
  },
}
</script>
