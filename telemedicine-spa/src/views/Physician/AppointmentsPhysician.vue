<template>
  <div class="physician-appointments p-6">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-1">
      Appointments
    </h1>
    <p class="text-slate-600 dark:text-slate-400 mb-8">
      Manage your consultation schedule
    </p>

    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent"></div>
    </div>

    <div v-else class="card-modern overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead>
            <tr class="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-700">
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">ID</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Service</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Date</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Status</th>
              <th class="p-4 text-left text-sm font-semibold text-slate-600 dark:text-slate-400">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="apt in appointments" :key="apt.id" class="border-b border-slate-100 dark:border-slate-700/50 hover:bg-slate-50/50">
              <td class="p-4 text-slate-600 dark:text-slate-400">{{ apt.id }}</td>
              <td class="p-4 font-medium text-slate-800 dark:text-slate-100">{{ apt.serviceName || 'Consultation' }}</td>
              <td class="p-4 text-slate-600 dark:text-slate-400">{{ formatDate(apt.appointmentDate || apt.scheduleStartTime) }}</td>
              <td class="p-4">
                <span :class="['px-3 py-1 rounded-full text-xs font-medium', statusClass(statusLabel(apt.status))]">
                  {{ statusLabel(apt.status) }}
                </span>
              </td>
              <td class="p-4">
                <button
                  v-if="statusLabel(apt.status) === 'Not attended'"
                  class="text-primary-600 hover:text-primary-700 font-medium"
                  @click="respondToAppointment(apt.id)"
                >
                  Respond
                </button>
                <span v-else class="text-slate-400 text-sm">{{ statusLabel(apt.status) }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="!appointments.length && !loading" class="p-12 text-center text-slate-500">
        No appointments yet
      </div>
    </div>
  </div>
</template>

<script>
import { getPhysicianAppointments, getCurrentPhysician, respondToAppointment } from '@/api'

export default {
  name: 'AppointmentsPhysician',
  data() {
    return {
      appointments: [],
      loading: false,
    }
  },
  methods: {
    formatDate(d) {
      if (!d) return 'N/A'
      const s = typeof d === 'string' ? d : d?.toString?.()
      return s?.split('T')[0] || 'N/A'
    },
    statusLabel(status) {
      if (typeof status === 'string') return status
      const labels = { 0: 'Active', 1: 'Not attended', 2: 'Attended', 3: 'Cancelled' }
      return labels[status] || 'Unknown'
    },
    statusClass(status) {
      const s = (status || '').toLowerCase()
      if (s === 'attended' || s.includes('active') || s.includes('completed')) return 'bg-emerald-100 text-emerald-600 dark:bg-emerald-900/40 dark:text-emerald-400'
      if (s.includes('not') || s.includes('unattended')) return 'bg-amber-100 text-amber-600 dark:bg-amber-900/40 dark:text-amber-400'
      if (s.includes('cancel')) return 'bg-red-100 text-red-600 dark:bg-red-900/40 dark:text-red-400'
      return 'bg-slate-100 text-slate-600 dark:bg-slate-700 dark:text-slate-300'
    },
    async respondToAppointment(id) {
      try {
        await respondToAppointment({ appointmentId: id })
        if (this.$notification) this.$notification.success('Responded', 'Appointment marked as active.')
        this.load()
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message || 'Could not respond.')
      }
    },
    async load() {
      this.loading = true
      try {
        const p = await getCurrentPhysician()
        if (p?.id) localStorage.setItem('physician-id', String(p.id))
        const id = localStorage.getItem('physician-id') || p?.id
        if (id) {
          const data = await getPhysicianAppointments(id)
          this.appointments = Array.isArray(data) ? data : (data?.data ?? [])
        }
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
      } finally {
        this.loading = false
      }
    },
  },
  mounted() {
    this.load()
  },
}
</script>
