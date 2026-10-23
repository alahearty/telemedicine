<template>
  <div class="max-w-6xl mx-auto p-6">
    <div class="text-center mb-8">
      <h1 class="font-display text-3xl font-bold text-slate-800 dark:text-slate-100">Book an appointment</h1>
      <p class="text-slate-600 dark:text-slate-400 mt-2">
        Choose a real service, physician and available slot. Times are displayed in your local time zone.
      </p>
    </div>

    <div v-if="profileError" class="mb-6 rounded-xl border border-amber-200 bg-amber-50 p-4 text-sm text-amber-800 dark:border-amber-800 dark:bg-amber-900/20 dark:text-amber-200">
      {{ profileError }}
      <router-link to="/dashboard/update-record" class="font-semibold underline ml-1">Complete your profile</router-link>
    </div>

    <div class="space-y-8">
      <section class="card-modern p-6">
        <h2 class="font-display text-xl font-semibold text-slate-800 dark:text-slate-100 mb-4">1. Select a service</h2>
        <div v-if="loadingServices" class="flex items-center gap-3 text-slate-500">
          <div class="animate-spin rounded-full h-6 w-6 border-2 border-primary-500 border-t-transparent"></div>
          <span>Loading services...</span>
        </div>
        <div v-else-if="!services.length" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300">
          No telemedicine services are available. Please contact an administrator.
        </div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <button
            v-for="service in services"
            :key="service.id"
            data-testid="service-card" type="button" class="text-left rounded-2xl border p-5 transition-all"
            :class="selectedService?.id === service.id
              ? 'border-primary-500 bg-primary-50 ring-2 ring-primary-200 dark:bg-primary-900/20 dark:ring-primary-800'
              : 'border-slate-200 bg-white hover:border-primary-300 dark:border-slate-700 dark:bg-slate-800'"
            @click="selectedService = service"
          >
            <div class="flex items-start justify-between gap-3">
              <div class="w-11 h-11 rounded-xl bg-primary-100 text-primary-600 flex items-center justify-center dark:bg-primary-900/40 dark:text-primary-300">
                <Icon :icon="serviceIcon(service.serviceName)" class="text-xl" />
              </div>
              <span class="text-lg font-bold text-primary-600 dark:text-primary-300">${{ Number(service.cost).toFixed(2) }}</span>
            </div>
            <h3 class="font-semibold text-slate-800 dark:text-slate-100 mt-4">{{ service.serviceName }}</h3>
            <p class="text-sm text-slate-500 dark:text-slate-400 mt-1">{{ service.description }}</p>
          </button>
        </div>
      </section>

      <section v-if="selectedService" class="card-modern p-6">
        <div class="flex flex-wrap items-center justify-between gap-3 mb-4">
          <h2 class="font-display text-xl font-semibold text-slate-800 dark:text-slate-100">2. Select a physician</h2>
          <input v-model="doctorSearch" type="search" class="input-modern max-w-xs" placeholder="Search name or speciality" />
        </div>
        <div v-if="loadingDoctors" class="flex items-center gap-3 text-slate-500">
          <div class="animate-spin rounded-full h-6 w-6 border-2 border-primary-500 border-t-transparent"></div>
          <span>Loading physicians...</span>
        </div>
        <div v-else-if="!filteredDoctors.length" class="rounded-xl border border-slate-200 bg-slate-50 p-4 text-sm text-slate-500 dark:border-slate-700 dark:bg-slate-800/50">
          No physicians match your search.
        </div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <button
            v-for="doctor in filteredDoctors"
            :key="doctor.id"
            data-testid="service-card" type="button" class="text-left rounded-2xl border p-5 transition-all"
            :class="selectedDoctor?.id === doctor.id
              ? 'border-primary-500 bg-primary-50 ring-2 ring-primary-200 dark:bg-primary-900/20 dark:ring-primary-800'
              : 'border-slate-200 bg-white hover:border-primary-300 dark:border-slate-700 dark:bg-slate-800'"
            @click="chooseDoctor(doctor)"
          >
            <div class="flex items-center gap-4">
              <div class="w-12 h-12 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center font-bold dark:bg-primary-900/40 dark:text-primary-300">
                {{ initials(doctor) }}
              </div>
              <div class="min-w-0">
                <h3 class="font-semibold text-slate-800 dark:text-slate-100">Dr. {{ doctor.firstName }} {{ doctor.lastName }}</h3>
                <p class="text-sm text-primary-600 dark:text-primary-300">{{ doctor.medicalSpecialization || 'General Practice' }}</p>
                <p class="text-xs text-slate-500 dark:text-slate-400 truncate">{{ doctor.bio || doctor.qualifications || 'Licensed telemedicine physician' }}</p>
              </div>
            </div>
          </button>
        </div>
      </section>

      <section v-if="selectedDoctor" class="card-modern p-6">
        <h2 class="font-display text-xl font-semibold text-slate-800 dark:text-slate-100 mb-1">3. Choose a slot</h2>
        <p class="text-sm text-slate-500 dark:text-slate-400 mb-4">
          Dr. {{ selectedDoctor.firstName }} {{ selectedDoctor.lastName }} · {{ selectedDoctor.medicalSpecialization }}
        </p>
        <SlotPicker
          :physician-id="selectedDoctor.id"
          :from-utc="slotFrom"
          :to-utc="slotTo"
          :selected-start="selectedSlot?.start || ''"
          @select="selectedSlot = $event"
        />
        <div v-if="selectedSlot" class="mt-5 rounded-xl bg-primary-50 dark:bg-primary-900/20 border border-primary-100 dark:border-primary-800 p-4 text-sm text-primary-800 dark:text-primary-200">
          Selected: <strong>{{ formatSlot(selectedSlot.start) }}</strong> — {{ selectedSlot.durationMinutes }} minutes
        </div>
      </section>

      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <p class="text-sm text-slate-500 dark:text-slate-400">
          The payment step is a clearly labelled sandbox. No card data is collected.
        </p>
        <button
          type="button"
          data-testid="continue-to-payment" class="btn-primary px-8 py-3"
          :disabled="!canContinue || loading"
          @click="continueToPayment"
        >
          {{ loading ? 'Preparing...' : 'Continue to payment' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { Icon } from '@iconify/vue'
import SlotPicker from '@/components/appointments/SlotPicker.vue'
import { getTelemedicineServices, getPhysicians, getCurrentPatient } from '@/api'

export default {
  name: 'BookAppointment',
  components: { Icon, SlotPicker },
  data() {
    return {
      services: [],
      doctors: [],
      loadingServices: false,
      loadingDoctors: false,
      loading: false,
      profileError: '',
      doctorSearch: '',
      selectedService: null,
      selectedDoctor: null,
      selectedSlot: null,
    }
  },
  computed: {
    filteredDoctors() {
      const term = this.doctorSearch.trim().toLowerCase()
      if (!term) return this.doctors
      return this.doctors.filter(d =>
        `${d.firstName} ${d.lastName}`.toLowerCase().includes(term)
        || (d.medicalSpecialization || '').toLowerCase().includes(term)
        || (d.bio || '').toLowerCase().includes(term)
      )
    },
    slotFrom() {
      return new Date().toISOString()
    },
    slotTo() {
      return new Date(Date.now() + 21 * 24 * 60 * 60 * 1000).toISOString()
    },
    canContinue() {
      return Boolean(this.selectedService && this.selectedDoctor && this.selectedSlot)
    },
  },
  methods: {
    initials(doctor) {
      return `${(doctor.firstName || '?')[0]}${(doctor.lastName || '?')[0]}`.toUpperCase()
    },
    serviceIcon(name) {
      const value = (name || '').toLowerCase()
      if (value.includes('cardio') || value.includes('heart')) return 'bxs:heart'
      if (value.includes('paed') || value.includes('child')) return 'bxs:baby-carriage'
      if (value.includes('skin') || value.includes('derm')) return 'bxs:bandage'
      return 'bxs:stethoscope'
    },
    formatSlot(value) {
      return new Date(value).toLocaleString(undefined, {
        weekday: 'short', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
      })
    },
    chooseDoctor(doctor) {
      this.selectedDoctor = doctor
      this.selectedSlot = null
      const preferred = this.preferredSlot()
      if (preferred && Number(preferred.physicianId) === doctor.id && preferred.slot?.start) {
        this.selectedSlot = preferred.slot
      }
    },
    preferredSlot() {
      try {
        const stored = sessionStorage.getItem('preferredSlot')
        if (!stored) return null
        sessionStorage.removeItem('preferredSlot')
        return JSON.parse(stored)
      } catch (e) {
        return null
      }
    },
    async loadProfile() {
      try {
        await getCurrentPatient()
      } catch (e) {
        this.profileError = e.message || 'A patient profile is required before booking.'
      }
    },
    async loadServices() {
      this.loadingServices = true
      try {
        const data = await getTelemedicineServices()
        this.services = (Array.isArray(data) ? data : []).map(s => ({ ...s, cost: Number(s.cost) || 0 }))
      } catch (e) {
        if (this.$notification) this.$notification.error('Services', e.message || 'Could not load services.')
      } finally {
        this.loadingServices = false
      }
    },
    async loadDoctors() {
      this.loadingDoctors = true
      try {
        const data = await getPhysicians()
        this.doctors = Array.isArray(data) ? data : (data?.data ?? [])
        const prefill = Number(this.$route.query.physicianId)
        if (prefill) {
          const match = this.doctors.find(d => d.id === prefill)
          if (match) {
            const preferred = this.preferredSlot()
            this.selectedDoctor = match
            if (preferred && Number(preferred.physicianId) === match.id && preferred.slot?.start) {
              this.selectedSlot = preferred.slot
            }
          }
        }
      } catch (e) {
        if (this.$notification) this.$notification.error('Physicians', e.message || 'Could not load physicians.')
      } finally {
        this.loadingDoctors = false
      }
    },
    continueToPayment() {
      if (!this.canContinue) return
      this.loading = true
      try {
        sessionStorage.setItem('pendingAppointment', JSON.stringify({
          serviceId: this.selectedService.id,
          serviceName: this.selectedService.serviceName,
          serviceDescription: this.selectedService.description,
          costValue: this.selectedService.cost,
          physicianId: this.selectedDoctor.id,
          physicianName: `Dr. ${this.selectedDoctor.firstName} ${this.selectedDoctor.lastName}`,
          physicianSpecialization: this.selectedDoctor.medicalSpecialization,
          startUtc: this.selectedSlot.start,
          endUtc: this.selectedSlot.end,
          durationMinutes: this.selectedSlot.durationMinutes,
        }))
        this.$router.push('/dashboard/payment')
      } finally {
        this.loading = false
      }
    },
  },
  mounted() {
    this.loadProfile()
    this.loadServices()
    this.loadDoctors()
  },
}
</script>
