<template>
  <div class="appointment-container">
    <div class="appointment-header">
      <h1 class="appointment-title">Book Your Appointment</h1>
      <p class="appointment-subtitle">Select a service and choose your preferred date and time</p>
    </div>

    <div class="appointment-content">
      <!-- Service Selection -->
      <div class="services-section">
        <h2 class="section-title">Select a Service</h2>
        <div v-if="servicesLoading" class="flex items-center justify-center gap-3 py-12">
          <div class="animate-spin rounded-full h-10 w-10 border-2 border-primary-500 border-t-transparent"></div>
          <span class="text-slate-500 dark:text-slate-400">Loading services...</span>
        </div>
        <div v-else class="services-grid">
          <div
            v-for="service in services"
            :key="service.id"
            :class="['service-card', { 'service-selected': selected === service.id }]"
            @click="handleClick(service.id)"
          >
            <div class="service-icon">
              <component :is="iconComponent(service.icon)" />
            </div>
            <h3 class="service-title">{{ service.title }}</h3>
            <p class="service-description">{{ service.description }}</p>
            <div class="service-footer">
              <span class="service-cost">{{ service.cost }}</span>
              <span :class="['service-badge', { 'service-badge-selected': selected === service.id }]">
                {{ selected === service.id ? 'Selected' : 'Select' }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Date and Time Selection -->
      <div v-if="selected" class="datetime-section">
        <h2 class="section-title">Choose Date & Time</h2>
        <div class="datetime-selector">
          <div class="date-selector">
            <label class="selector-label">Select Date</label>
            <input 
              type="date" 
              v-model="selectedDate"
              :min="minDate"
              class="date-input"
            />
          </div>
          <div class="time-selector">
            <label class="selector-label">Select Time</label>
            <select v-model="selectedTime" class="time-select">
              <option value="">Choose time</option>
              <option v-for="time in availableTimes" :key="time" :value="time">
                {{ time }}
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Doctor Selection -->
      <div v-if="selected && selectedDate && selectedTime && selectedDoctor === null" class="doctor-section">
        <h2 class="section-title">Select a Doctor</h2>
        <div class="doctors-grid">
          <div
            v-for="doctor in availableDoctors"
            :key="doctor.id"
            :class="['doctor-card', { 'doctor-selected': selectedDoctor === doctor.id }]"
            @click="selectDoctor(doctor.id)"
          >
            <img :src="doctor.avatar" :alt="doctor.name" class="doctor-avatar" />
            <div class="doctor-info">
              <h3 class="doctor-name">{{ doctor.name }}</h3>
              <p class="doctor-specialty">{{ doctor.specialty }}</p>
              <div class="doctor-rating">
                <span class="rating-stars">★★★★★</span>
                <span class="rating-value">{{ doctor.rating }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

      <!-- Action Buttons -->
      <div class="appointment-actions">
        <button 
          :disabled="!selected || !selectedDate || !selectedTime || loading"
          class="continue-btn"
          :class="{ 'continue-btn-active': canContinue && !loading }"
          @click="confirmAppointment"
        >
          <span v-if="loading">Processing...</span>
          <span v-else-if="!canContinue">Complete Selection</span>
          <span v-else>Continue to Payment</span>
          <svg v-if="!loading" class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
        </button>
      </div>
  </div>
</template>

<script>
import { getTelemedicineServices, getPhysicians } from '@/api'
import EyeIcon from '@/components/icons/EyeIcon.vue'
import BrainIcon from '@/components/icons/BrainIcon.vue'
import CardiacIcon from '@/components/icons/CardiacIcon.vue'
import EarIcon from '@/components/icons/EarIcon.vue'
import OsteoporosisiIcon from '@/components/icons/OsteoporosisiIcon.vue'
import BagIcon from '@/components/icons/BagIcon.vue'

const iconMap = {
  'EyeIcon': EyeIcon,
  'BrainIcon': BrainIcon,
  'CardiacIcon': CardiacIcon,
  'EarIcon': EarIcon,
  'OsteoporosisiIcon': OsteoporosisiIcon,
  'BagIcon': BagIcon
}

export default {
  name: 'BookAppointment',
  components: {
    EyeIcon,
    BrainIcon,
    CardiacIcon,
    EarIcon,
    OsteoporosisiIcon,
    BagIcon
  },
  data() {
    return {
      selected: null,
      selectedDate: '',
      selectedTime: '',
      selectedDoctor: null,
      services: [],
      servicesLoading: false,
      loading: false,
      availableTimes: [
        '09:00 AM', '09:30 AM', '10:00 AM', '10:30 AM',
        '11:00 AM', '11:30 AM', '12:00 PM', '12:30 PM',
        '02:00 PM', '02:30 PM', '03:00 PM', '03:30 PM',
        '04:00 PM', '04:30 PM', '05:00 PM'
      ],
      availableDoctors: []
    }
  },
  computed: {
    minDate() {
      const tomorrow = new Date()
      tomorrow.setDate(tomorrow.getDate() + 1)
      return tomorrow.toISOString().split('T')[0]
    },
    canContinue() {
      return this.selected && this.selectedDate && this.selectedTime
    }
  },
  methods: {
    async fetchServices() {
      this.servicesLoading = true
      const timeout = (ms) => new Promise((_, reject) => setTimeout(() => reject(new Error('Request timeout')), ms))
      try {
        const response = await Promise.race([
          getTelemedicineServices(),
          timeout(4000)
        ])
        const list = Array.isArray(response) ? response : (response?.data ?? [])
        this.services = (list.length ? list : []).map(service => ({
          id: service.id,
          title: service.serviceName || service.name || 'Service',
          cost: `$${service.cost ?? 0}`,
          costValue: Number(service.cost) || 0,
          description: service.description || 'Telemedicine service',
          icon: this.mapServiceToIcon(service.serviceName || service.name || '')
        }))
        if (!this.services.length) throw new Error('No services')
      } catch (error) {
        console.warn('Services fetch failed, using defaults:', error?.message)
        this.services = [
          { id: 1, title: 'Eye Care', cost: '$200', description: 'Comprehensive eye examination and vision care services.', icon: 'EyeIcon' },
          { id: 2, title: 'Neurology', cost: '$200', description: 'Expert neurological consultation and treatment.', icon: 'BrainIcon' },
          { id: 3, title: 'Cardiac Care', cost: '$300', description: 'Heart health assessment and cardiovascular care.', icon: 'CardiacIcon' },
        ]
        if (this.$notification) this.$notification.warning('Using default services', 'Could not load from server. Ensure the API is running.')
      } finally {
        this.servicesLoading = false
      }
    },
    async fetchDoctors() {
      try {
        const data = await getPhysicians()
        const list = Array.isArray(data) ? data : (data?.data ?? [])
        this.availableDoctors = list.map((p, i) => ({
          id: p.id,
          name: `Dr. ${p.firstName || ''} ${p.lastName || ''}`.trim() || 'Physician',
          specialty: p.medicalSpecialization || p.specialty || 'General Practice',
          rating: 4.5,
          avatar: ['https://cdn.pixabay.com/photo/2016/11/29/13/14/attractive-1869761__340.jpg', 'https://cdn.pixabay.com/photo/2018/09/12/12/14/man-3672010__340.jpg', 'https://cdn.pixabay.com/photo/2018/01/15/07/51/woman-3083383__340.jpg'][i % 3]
        }))
        if (!this.availableDoctors.length) {
          this.availableDoctors = [
            { id: 1, name: 'Dr. Sarah Johnson', specialty: 'Cardiologist', rating: 4.8, avatar: 'https://cdn.pixabay.com/photo/2016/11/29/13/14/attractive-1869761__340.jpg' },
            { id: 2, name: 'Dr. Michael Chen', specialty: 'Neurologist', rating: 4.9, avatar: 'https://cdn.pixabay.com/photo/2018/09/12/12/14/man-3672010__340.jpg' },
            { id: 3, name: 'Dr. Emily Watson', specialty: 'Ophthalmologist', rating: 4.7, avatar: 'https://cdn.pixabay.com/photo/2018/01/15/07/51/woman-3083383__340.jpg' }
          ]
        }
      } catch (e) {
        this.availableDoctors = [
          { id: 1, name: 'Dr. Sarah Johnson', specialty: 'Cardiologist', rating: 4.8, avatar: 'https://cdn.pixabay.com/photo/2016/11/29/13/14/attractive-1869761__340.jpg' },
          { id: 2, name: 'Dr. Michael Chen', specialty: 'Neurologist', rating: 4.9, avatar: 'https://cdn.pixabay.com/photo/2018/09/12/12/14/man-3672010__340.jpg' },
          { id: 3, name: 'Dr. Emily Watson', specialty: 'Ophthalmologist', rating: 4.7, avatar: 'https://cdn.pixabay.com/photo/2018/01/15/07/51/woman-3083383__340.jpg' }
        ]
      }
    },
    iconComponent(iconName) {
      return iconMap[iconName] || BagIcon
    },
    mapServiceToIcon(serviceName) {
      const name = serviceName.toLowerCase()
      if (name.includes('eye') || name.includes('vision')) return 'EyeIcon'
      if (name.includes('brain') || name.includes('neuro')) return 'BrainIcon'
      if (name.includes('heart') || name.includes('cardiac')) return 'CardiacIcon'
      if (name.includes('ear') || name.includes('ent')) return 'EarIcon'
      if (name.includes('physio') || name.includes('therapy')) return 'OsteoporosisiIcon'
      if (name.includes('dental') || name.includes('tooth')) return 'BagIcon'
      return 'BagIcon'
    },
    handleClick(id) {
      if (this.selected === id) {
        this.selected = null
        this.selectedDate = ''
        this.selectedTime = ''
        this.selectedDoctor = null
      } else {
        this.selected = id
      }
    },
    selectDoctor(doctorId) {
      this.selectedDoctor = doctorId
    },
    async confirmAppointment() {
      if (!this.canContinue) return
      this.loading = true
      try {
        const appointmentData = {
          serviceId: this.selected,
          selectedDate: this.selectedDate,
          selectedTime: this.selectedTime,
          selectedDoctor: this.selectedDoctor,
          service: this.services.find(s => s.id === this.selected)
        }
        sessionStorage.setItem('pendingAppointment', JSON.stringify(appointmentData))
        this.$router.push('/dashboard/payment')
      } catch (error) {
        console.error('Error confirming appointment:', error)
        if (this.$notification) {
          this.$notification.error('Error', 'Failed to process appointment. Please try again.')
        }
      } finally {
        this.loading = false
      }
    }
  },
  async mounted() {
    await this.fetchServices()
    await this.fetchDoctors()
  }
}
</script>

<style scoped>
.appointment-container {
  @apply max-w-5xl mx-auto p-6 bg-slate-50 dark:bg-slate-900 min-h-screen;
}

.appointment-header {
  @apply text-center mb-10;
}

.appointment-title {
  @apply font-display text-3xl font-bold text-slate-800 dark:text-slate-100 mb-2;
}

.appointment-subtitle {
  @apply text-slate-600 dark:text-slate-400;
}

.appointment-content {
  @apply space-y-8;
}

.services-grid {
  @apply grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6;
}

.datetime-section,
.doctor-section {
  @apply bg-white dark:bg-slate-800 rounded-2xl shadow-soft border border-slate-100 dark:border-slate-700/50 p-6;
}

.datetime-selector {
  @apply grid grid-cols-1 md:grid-cols-2 gap-6;
}

.appointment-actions {
  @apply mt-8 flex justify-center;
}

.continue-btn {
  @apply px-8 py-3 rounded-xl font-semibold transition-all disabled:cursor-not-allowed flex items-center bg-slate-200 dark:bg-slate-700 text-slate-500;
}

.section-title {
  @apply font-display text-xl font-semibold text-slate-800 dark:text-slate-100 mb-4;
}

.service-card {
  @apply bg-white dark:bg-slate-800 rounded-2xl shadow-soft border border-slate-100 dark:border-slate-700/50 p-6 cursor-pointer border-2 border-transparent transition-all duration-300 hover:shadow-soft-lg;
}

.service-card.service-selected {
  @apply border-primary-500 bg-primary-50 dark:bg-primary-900/20 ring-2 ring-primary-200 dark:ring-primary-800;
}

.service-icon {
  @apply w-14 h-14 bg-primary-100 dark:bg-primary-900/40 rounded-2xl flex items-center justify-center mb-4 text-primary-600 dark:text-primary-400;
}

.service-cost {
  @apply text-xl font-bold text-primary-600 dark:text-primary-400;
}

.service-badge-selected {
  @apply bg-primary-500 text-white;
}

.date-input,
.time-select {
  @apply w-full px-4 py-3 rounded-xl border border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-800 text-slate-800 dark:text-slate-200 focus:ring-2 focus:ring-primary-500/30 focus:border-primary-500;
}

.doctor-card.doctor-selected {
  @apply border-primary-500 bg-primary-50 dark:bg-primary-900/20;
}

.continue-btn-active {
  @apply px-6 py-2.5 rounded-xl font-semibold text-white bg-primary-500 hover:bg-primary-600 transition-all duration-200;
}
</style>
