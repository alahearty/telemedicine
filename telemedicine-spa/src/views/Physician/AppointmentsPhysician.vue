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
                <div class="flex flex-wrap items-center gap-3">
                  <router-link :to="`/physician/appointments/${apt.id}`" class="text-primary-600 hover:text-primary-700 font-medium">Details</router-link>
                  <template v-if="statusLabel(apt.status) === 'Pending'">
                    <button class="text-emerald-600 hover:text-emerald-700 font-medium" @click="respond(apt.id, true)">Accept</button>
                    <button class="text-red-600 hover:text-red-700 font-medium" @click="respond(apt.id, false)">Decline</button>
                  </template>
                </div>
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
import { statusClass as classForStatus, statusLabel as labelForStatus } from '@/utils/appointments'

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
      return labelForStatus(status)
    },
    statusClass(status) {
      return classForStatus(status)
    },
    async respond(id, accept) {
      let reason = null
      if (!accept) {
        reason = window.prompt('Reason for declining this appointment:')
        if (reason === null) return
        if (!reason.trim()) {
          if (this.$notification) this.$notification.error('Decline reason required', 'Enter a reason before declining.')
          return
        }
      }
      try {
        await respondToAppointment({ appointmentId: id, accept, reason: reason ? reason.trim() : null })
        if (this.$notification) this.$notification.success(accept ? 'Accepted' : 'Declined', accept ? 'The appointment is now active.' : 'The appointment was declined.')
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
