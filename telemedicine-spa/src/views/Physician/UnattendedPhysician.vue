<template>
  <div class="physician-unattended p-6">
    <h1 class="font-display text-2xl font-bold text-slate-800 dark:text-slate-100 mb-1">
      Unattended Appointments
    </h1>
    <p class="text-slate-600 dark:text-slate-400 mb-8">
      New appointment requests awaiting your response
    </p>

    <div v-if="loading" class="flex justify-center py-12">
      <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent"></div>
    </div>

    <div v-else class="space-y-4">
      <div
        v-for="apt in unattended"
        :key="apt.id"
        class="card-modern p-6 flex items-center justify-between flex-wrap gap-4"
      >
        <div>
          <p class="font-medium text-slate-800 dark:text-slate-100">{{ apt.serviceName || apt.telemedicineService?.serviceName || 'Consultation' }}</p>
          <p class="text-sm text-slate-500">ID: {{ apt.id }} • {{ formatDate(apt.appointmentDate || apt.scheduleStartTime) }}</p>
        </div>
        <button
          class="btn-primary"
          @click="respondToAppointment(apt.id)"
        >
          Respond & Accept
        </button>
      </div>
      <div v-if="!unattended.length && !loading" class="card-modern p-12 text-center text-slate-500">
        <Icon icon="bxs:bell" class="text-4xl mb-4 opacity-50" />
        <p>No unattended appointments</p>
      </div>
    </div>
  </div>
</template>

<script>
import { Icon } from '@iconify/vue'
import { getUnattendedAppointments, respondToAppointment } from '@/api'

export default {
  name: 'UnattendedPhysician',
  components: { Icon },
  data() {
    return {
      unattended: [],
      loading: false,
    }
  },
  methods: {
    formatDate(d) {
      if (!d) return 'N/A'
      const s = typeof d === 'string' ? d : d?.toString?.()
      return s?.split('T')[0] || 'N/A'
    },
    async respondToAppointment(id) {
      try {
        await respondToAppointment({ appointmentId: id })
        if (this.$notification) this.$notification.success('Accepted', 'Appointment marked as active.')
        this.load()
      } catch (e) {
        if (this.$notification) this.$notification.error('Error', e.message)
      }
    },
    async load() {
      this.loading = true
      try {
        const data = await getUnattendedAppointments()
        this.unattended = Array.isArray(data) ? data : (data?.data ?? [])
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
