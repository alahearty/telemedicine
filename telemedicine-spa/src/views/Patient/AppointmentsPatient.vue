<template>
  <div class="max-w-5xl mx-auto p-6">
    <div class="flex flex-wrap items-end justify-between gap-3 mb-6">
      <div>
        <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100">My appointments</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Upcoming consultations, history and cancellations.</p>
      </div>
      <router-link to="/dashboard/book-service" class="btn-primary">Book appointment</router-link>
    </div>

    <div class="flex gap-2 mb-6">
      <button v-for="tab in tabs" :key="tab.key" type="button" class="px-4 py-2 rounded-xl text-sm font-medium transition-colors"
        :class="activeTab === tab.key ? 'bg-primary-500 text-white' : 'bg-white text-slate-600 border border-slate-200 dark:bg-slate-800 dark:text-slate-300 dark:border-slate-700'"
        @click="activeTab = tab.key">
        {{ tab.label }}
      </button>
    </div>

    <div v-if="loading" class="flex items-center gap-3 py-16 text-slate-500">
      <div class="animate-spin rounded-full h-8 w-8 border-2 border-primary-500 border-t-transparent"></div>
      <span>Loading appointments...</span>
    </div>

    <div v-else-if="error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300">
      {{ error }}
    </div>

    <div v-else-if="!filtered.length" class="card-modern p-12 text-center text-slate-500">
      <Icon icon="bxs:calendar-x" class="text-4xl mb-3 opacity-50" />
      <p>No {{ activeTab === 'upcoming' ? 'upcoming' : activeTab }} appointments.</p>
      <router-link v-if="activeTab === 'upcoming'" to="/dashboard/book-service" class="text-primary-600 font-medium mt-3 inline-block">Book an appointment</router-link>
    </div>

    <div v-else class="space-y-4">
      <article v-for="apt in filtered" :key="apt.id" class="card card-hover p-5">
        <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div class="min-w-0">
            <div class="flex flex-wrap items-center gap-2">
              <p class="font-semibold text-slate-800 dark:text-slate-100">{{ apt.serviceName || 'Consultation' }}</p>
              <StatusBadge :status="apt.status" />
            </div>
            <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">
              {{ formatDateTime(apt.appointmentDate || apt.scheduleStartTime) }}
            </p>
            <p v-if="apt.physicianName" class="mt-1 text-xs text-slate-500 dark:text-slate-400">
              Dr. {{ apt.physicianName }}<span v-if="apt.physicianSpecialization"> · {{ apt.physicianSpecialization }}</span>
            </p>
          </div>
          <div class="flex flex-wrap items-center gap-2 sm:justify-end">
            <AppointmentActions :appointment="apt" compact />
            <router-link :to="`/dashboard/appointments/${apt.id}`" class="btn-ghost px-2.5 py-1.5 text-xs">
              Details
              <Icon icon="arrow-right" class="text-xs" />
            </router-link>
          </div>
        </div>
      </article>
    </div>
  </div>
</template>

<script>
import Icon from '@/components/LocalIcon.vue'
import AppointmentActions from '@/components/appointments/AppointmentActions.vue'
import StatusBadge from '@/components/ui/StatusBadge.vue'
import { getCurrentPatient, getPatientAppointments } from '@/api'
import { formatDateTime, isUpcoming, statusLabel } from '@/utils/appointments'

export default {
  name: 'AppointmentsPatient',
  components: { AppointmentActions, Icon, StatusBadge },
  data() {
    return {
      appointments: [],
      loading: true,
      error: '',
      activeTab: 'upcoming',
      tabs: [
        { key: 'upcoming', label: 'Upcoming' },
        { key: 'history', label: 'History' },
        { key: 'cancelled', label: 'Cancelled / declined' },
      ],
    }
  },
  computed: {
    filtered() {
      return this.appointments.filter(apt => {
        const status = statusLabel(apt.status).toLowerCase()
        if (this.activeTab === 'upcoming') return isUpcoming(apt.status)
        if (this.activeTab === 'history') return status === 'attended'
        return status === 'cancelled' || status === 'declined'
      })
    },
  },
  methods: {
    formatDateTime,
    statusLabel,
    async load() {
      this.loading = true
      this.error = ''
      try {
        const patient = await getCurrentPatient()
        const id = patient?.id ?? patient?.data?.id
        if (!id) throw new Error('Patient profile not found.')
        const data = await getPatientAppointments(id)
        this.appointments = Array.isArray(data) ? data : (data?.data ?? [])
      } catch (e) {
        this.error = e.message || 'Could not load appointments.'
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
