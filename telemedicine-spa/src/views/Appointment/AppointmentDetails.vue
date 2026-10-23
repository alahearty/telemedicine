<template>
  <div class="max-w-4xl mx-auto p-6">
    <div class="flex flex-wrap items-center justify-between gap-3 mb-6">
      <div>
        <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100">Appointment details</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Reference #{{ id }}</p>
      </div>
      <router-link :to="backPath" class="btn-secondary">Back to appointments</router-link>
    </div>

    <div v-if="loading" class="flex items-center gap-3 py-16 text-slate-500">
      <div class="animate-spin rounded-full h-8 w-8 border-2 border-primary-500 border-t-transparent"></div>
      <span>Loading appointment...</span>
    </div>

    <div v-else-if="error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300">
      {{ error }}
    </div>

    <template v-else-if="appointment">
      <div class="card-modern p-6">
        <div class="flex flex-wrap items-center justify-between gap-3 mb-5">
          <h2 class="text-xl font-semibold text-slate-800 dark:text-slate-100">{{ appointment.serviceName || 'Consultation' }}</h2>
          <span class="px-3 py-1 rounded-full text-sm font-medium" :class="statusClass(appointment.status)">{{ statusLabel(appointment.status) }}</span>
        </div>

        <dl class="grid grid-cols-1 sm:grid-cols-2 gap-x-8 gap-y-4 text-sm">
          <div><dt class="text-slate-500">Start</dt><dd class="font-medium text-slate-800 dark:text-slate-100">{{ formatDateTime(appointment.appointmentDate || appointment.scheduleStartTime) }}</dd></div>
          <div><dt class="text-slate-500">End</dt><dd class="font-medium text-slate-800 dark:text-slate-100">{{ formatDateTime(appointment.scheduleEndTime) }}</dd></div>
          <div><dt class="text-slate-500">Physician</dt><dd class="font-medium text-slate-800 dark:text-slate-100">Dr. {{ appointment.physicianName || 'Not assigned' }}<span v-if="appointment.physicianSpecialization" class="text-slate-500"> · {{ appointment.physicianSpecialization }}</span></dd></div>
          <div v-if="isPhysicianOrAdmin"><dt class="text-slate-500">Patient</dt><dd class="font-medium text-slate-800 dark:text-slate-100">{{ appointment.patientName || 'Not recorded' }}</dd></div>
          <div v-if="isPhysicianOrAdmin"><dt class="text-slate-500">Patient contact</dt><dd class="font-medium text-slate-800 dark:text-slate-100">{{ appointment.patientEmail }}<span v-if="appointment.patientPhone"> · {{ appointment.patientPhone }}</span></dd></div>
          <div class="sm:col-span-2"><dt class="text-slate-500">Service</dt><dd class="text-slate-700 dark:text-slate-300">{{ appointment.serviceDescription || 'Telemedicine consultation' }}</dd></div>
          <div v-if="appointment.notes" class="sm:col-span-2"><dt class="text-slate-500">Notes</dt><dd class="text-slate-700 dark:text-slate-300">{{ appointment.notes }}</dd></div>
          <div v-if="appointment.statusReason" class="sm:col-span-2"><dt class="text-slate-500">Reason</dt><dd class="text-slate-700 dark:text-slate-300">{{ appointment.statusReason }}</dd></div>
        </dl>
      </div>

      <div class="card-modern p-6 mt-6">
        <h3 class="font-semibold text-slate-800 dark:text-slate-100 mb-4">Actions</h3>
        <p v-if="actionError" class="rounded-xl border border-red-200 bg-red-50 p-3 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300 mb-4">{{ actionError }}</p>
        <p v-if="actionMessage" class="rounded-xl border border-green-200 bg-green-50 p-3 text-sm text-green-700 dark:border-green-800 dark:bg-green-900/20 dark:text-green-300 mb-4">{{ actionMessage }}</p>

        <div class="flex flex-wrap gap-3">
          <template v-if="isPhysician && isPending">
            <button type="button" class="btn-primary" :disabled="busy" @click="respond(true)">Accept appointment</button>
            <button type="button" class="btn-secondary text-red-600 border-red-200" :disabled="busy" @click="decline">Decline with reason</button>
          </template>

          <button v-if="isPatient && canPatientReschedule" type="button" class="btn-secondary" :disabled="busy" @click="showReschedule = !showReschedule">
            {{ showReschedule ? 'Hide reschedule' : 'Reschedule' }}
          </button>

          <button v-if="canCancel" type="button" class="btn-secondary text-red-600 border-red-200" :disabled="busy" @click="cancel">
            {{ isPhysician ? 'Cancel appointment' : 'Cancel appointment' }}
          </button>
        </div>

        <div v-if="!isPending && !canPatientReschedule && !canCancel" class="text-sm text-slate-500">
          No actions are available for an appointment with status {{ statusLabel(appointment.status) }}.
        </div>

        <div v-if="showReschedule" class="mt-6 border-t border-slate-100 dark:border-slate-700 pt-5">
          <h4 class="font-semibold text-slate-800 dark:text-slate-100 mb-1">Choose a new slot</h4>
          <p class="text-sm text-slate-500 dark:text-slate-400 mb-4">The current slot will be released when the new slot is saved.</p>
          <SlotPicker
            :physician-id="appointment.physicianId"
            :from-utc="fromUtc"
            :to-utc="toUtc"
            :selected-start="newSlot?.start || ''"
            @select="newSlot = $event"
          />
          <button v-if="newSlot" type="button" class="btn-primary mt-4" :disabled="busy" @click="reschedule">
            Confirm reschedule to {{ formatDateTime(newSlot.start) }}
          </button>
        </div>
      </div>
    </template>
  </div>
</template>

<script>
import SlotPicker from '@/components/appointments/SlotPicker.vue'
import { getAppointment, updateAppointment, cancelAppointment, respondToAppointment } from '@/api'
import { formatDateTime, statusClass, statusLabel } from '@/utils/appointments'

export default {
  name: 'AppointmentDetails',
  components: { SlotPicker },
  data() {
    return {
      id: Number(this.$route.params.id),
      appointment: null,
      loading: true,
      error: '',
      busy: false,
      actionError: '',
      actionMessage: '',
      showReschedule: false,
      newSlot: null,
      role: (localStorage.getItem('user-role') || '').toLowerCase(),
    }
  },
  computed: {
    isPatient() { return this.role.includes('patient') },
    isPhysician() { return this.role.includes('physician') || this.role.includes('doctor') },
    isAdmin() { return this.role.includes('admin') || this.role.includes('administrator') },
    isPhysicianOrAdmin() { return this.isPhysician || this.isAdmin },
    isPending() { return statusLabel(this.appointment?.status).toLowerCase() === 'pending' },
    canPatientReschedule() {
      const status = statusLabel(this.appointment?.status).toLowerCase()
      return status === 'pending' || status === 'active' || status === 'not attended'
    },
    canCancel() {
      const status = statusLabel(this.appointment?.status).toLowerCase()
      if (status === 'attended' || status === 'cancelled' || status === 'declined') return false
      return this.isPatient || this.isPhysician || this.isAdmin
    },
    backPath() {
      return this.isPhysician ? '/physician/appointments' : '/dashboard/appointments'
    },
    fromUtc() { return new Date().toISOString() },
    toUtc() { return new Date(Date.now() + 21 * 24 * 60 * 60 * 1000).toISOString() },
  },
  methods: {
    formatDateTime,
    statusClass,
    statusLabel,
    async load() {
      this.loading = true
      this.error = ''
      try {
        const data = await getAppointment(this.id)
        this.appointment = data?.data ?? data
      } catch (e) {
        this.error = e.message || 'Could not load this appointment.'
      } finally {
        this.loading = false
      }
    },
    async respond(accept) {
      this.busy = true
      this.actionError = ''
      this.actionMessage = ''
      try {
        await respondToAppointment({ appointmentId: this.id, accept })
        this.actionMessage = accept ? 'Appointment accepted.' : 'Appointment declined.'
        await this.load()
      } catch (e) {
        this.actionError = e.message || 'Could not respond to the appointment.'
      } finally {
        this.busy = false
      }
    },
    async decline() {
      const reason = window.prompt('Reason for declining this appointment:')
      if (reason === null) return
      if (!reason.trim()) {
        this.actionError = 'A decline reason is required.'
        return
      }
      this.busy = true
      this.actionError = ''
      this.actionMessage = ''
      try {
        await respondToAppointment({ appointmentId: this.id, accept: false, reason: reason.trim() })
        this.actionMessage = 'Appointment declined.'
        await this.load()
      } catch (e) {
        this.actionError = e.message || 'Could not decline the appointment.'
      } finally {
        this.busy = false
      }
    },
    async cancel() {
      const reason = window.prompt('Reason for cancelling (optional):', '') ?? ''
      this.busy = true
      this.actionError = ''
      this.actionMessage = ''
      try {
        await cancelAppointment(this.id, reason.trim())
        this.actionMessage = 'Appointment cancelled.'
        await this.load()
      } catch (e) {
        this.actionError = e.message || 'Could not cancel the appointment.'
      } finally {
        this.busy = false
      }
    },
    async reschedule() {
      if (!this.newSlot) return
      this.busy = true
      this.actionError = ''
      this.actionMessage = ''
      try {
        await updateAppointment(this.id, {
          appointmentDate: this.newSlot.start,
          durationMinutes: this.newSlot.durationMinutes,
        })
        this.actionMessage = 'Appointment rescheduled.'
        this.showReschedule = false
        this.newSlot = null
        await this.load()
      } catch (e) {
        this.actionError = e.message || 'Could not reschedule the appointment.'
      } finally {
        this.busy = false
      }
    },
  },
  watch: {
    '$route.params.id'(value) {
      if (!value) return
      this.id = Number(value)
      this.newSlot = null
      this.showReschedule = false
      this.load()
    },
  },
  mounted() {
    this.load()
  },
}
</script>
