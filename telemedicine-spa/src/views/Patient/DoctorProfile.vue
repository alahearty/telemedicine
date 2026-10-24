<template>
  <div class="max-w-5xl mx-auto p-6">
    <div v-if="loading" class="flex items-center gap-3 py-16 text-slate-500">
      <div class="animate-spin rounded-full h-8 w-8 border-2 border-primary-500 border-t-transparent"></div>
      <span>Loading profile...</span>
    </div>

    <div v-else-if="error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300">
      {{ error }}
    </div>

    <template v-else-if="doctor">
      <div class="card-modern p-8">
        <div class="flex flex-col sm:flex-row items-start gap-6">
          <div class="w-20 h-20 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center text-2xl font-bold dark:bg-primary-900/40 dark:text-primary-300">
            {{ initials }}
          </div>
          <div class="flex-1">
            <h1 class="font-display text-3xl font-bold text-slate-800 dark:text-slate-100">Dr. {{ doctor.firstName }} {{ doctor.lastName }}</h1>
            <p class="text-primary-600 dark:text-primary-300 font-medium mt-1">{{ doctor.medicalSpecialization || 'General Practice' }}</p>
            <p class="text-sm text-slate-500 dark:text-slate-400 mt-1">Licence {{ doctor.license || 'not recorded' }} · {{ doctor.email }}</p>
            <p class="text-slate-600 dark:text-slate-300 mt-4">{{ doctor.bio || 'No biography has been provided.' }}</p>
            <p v-if="doctor.qualifications" class="text-sm text-slate-500 dark:text-slate-400 mt-2"><strong>Qualifications:</strong> {{ doctor.qualifications }}</p>
            <div class="mt-6 flex flex-wrap gap-3">
              <router-link :to="`/dashboard/book-service?physicianId=${doctor.id}`" class="btn-primary">Book with this doctor</router-link>
              <router-link to="/dashboard/doctors" class="btn-secondary">Back to directory</router-link>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-6">
        <section class="card-modern p-6">
          <h2 class="font-display text-lg font-semibold text-slate-800 dark:text-slate-100 mb-4">Weekly availability</h2>
          <div v-if="!availability.length" class="text-sm text-slate-500">No working hours have been published.</div>
          <dl v-else class="space-y-2 text-sm">
            <div v-for="rule in availability" :key="rule.id" class="flex justify-between gap-3">
              <dt class="text-slate-500">{{ dayName(rule.dayOfWeek) }}</dt>
              <dd class="font-medium text-slate-700 dark:text-slate-200">{{ shortTime(rule.startTime) }} – {{ shortTime(rule.endTime) }}</dd>
            </div>
          </dl>
          <p class="text-xs text-slate-400 mt-4">Schedule is displayed in UTC. Slots below are converted to your local time.</p>
        </section>

        <section class="card-modern p-6 lg:col-span-2">
          <h2 class="font-display text-lg font-semibold text-slate-800 dark:text-slate-100 mb-1">Next available slots</h2>
          <p class="text-sm text-slate-500 dark:text-slate-400 mb-4">Choose a slot to start a booking.</p>
          <SlotPicker
            :physician-id="doctor.id"
            :from-utc="fromUtc"
            :to-utc="toUtc"
            :selected-start="selectedSlot?.start || ''"
            @select="selectedSlot = $event"
          />
          <div v-if="selectedSlot" class="mt-5 flex flex-wrap items-center justify-between gap-3 rounded-xl border border-primary-100 bg-primary-50 p-4 dark:border-primary-800 dark:bg-primary-900/20">
            <span class="text-sm text-primary-800 dark:text-primary-200">
              Selected {{ localDateTime(selectedSlot.start) }}
            </span>
            <button type="button" data-testid="continue-from-profile" class="btn-primary" @click="bookSelected">Continue booking</button>
          </div>
        </section>
      </div>
    </template>
  </div>
</template>

<script>
import SlotPicker from '@/components/appointments/SlotPicker.vue'
import { getPhysician, getPhysicianAvailability } from '@/api'

export default {
  name: 'DoctorProfile',
  components: { SlotPicker },
  data() {
    return {
      doctor: null,
      availability: [],
      loading: true,
      error: '',
      selectedSlot: null,
    }
  },
  computed: {
    initials() {
      if (!this.doctor) return '?'
      return `${(this.doctor.firstName || '?')[0]}${(this.doctor.lastName || '?')[0]}`.toUpperCase()
    },
    fromUtc() {
      return new Date().toISOString()
    },
    toUtc() {
      return new Date(Date.now() + 21 * 24 * 60 * 60 * 1000).toISOString()
    },
  },
  methods: {
    dayName(day) {
      const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']
      return days[Number(day)] || day
    },
    shortTime(value) {
      if (!value) return ''
      return String(value).slice(0, 5)
    },
    localDateTime(value) {
      return new Date(value).toLocaleString()
    },
    async load() {
      this.loading = true
      this.error = ''
      try {
        const id = Number(this.$route.params.id)
        const data = await getPhysician(id)
        this.doctor = data?.data ?? data
        const rules = await getPhysicianAvailability(id)
        this.availability = Array.isArray(rules) ? rules : []
      } catch (e) {
        this.error = e.message || 'Could not load this doctor.'
      } finally {
        this.loading = false
      }
    },
    bookSelected() {
      if (!this.selectedSlot) return
      sessionStorage.setItem('preferredSlot', JSON.stringify({
        physicianId: this.doctor.id,
        slot: this.selectedSlot,
      }))
      this.$router.push({ path: '/dashboard/book-service', query: { physicianId: this.doctor.id } })
    },
  },
  mounted() {
    this.load()
  },
}
</script>
