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
        <div class="services-grid">
          <div
            v-for="service in services"
            :key="service.id"
            :class="['service-card', { 'service-selected': selected === service.id }]"
            @click="handleClick(service.id)"
          >
            <div class="service-icon">
              <component :is="iconMap[service.icon] || BagIcon" />
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
import { getTelemedicineServices } from '@/api'
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
      loading: false,
      availableTimes: [
        '09:00 AM', '09:30 AM', '10:00 AM', '10:30 AM',
        '11:00 AM', '11:30 AM', '12:00 PM', '12:30 PM',
        '02:00 PM', '02:30 PM', '03:00 PM', '03:30 PM',
        '04:00 PM', '04:30 PM', '05:00 PM'
      ],
      availableDoctors: [
        {
          id: 1,
          name: 'Dr. Sarah Johnson',
          specialty: 'Cardiologist',
          rating: 4.8,
          avatar: 'https://cdn.pixabay.com/photo/2016/11/29/13/14/attractive-1869761__340.jpg'
        },
        {
          id: 2,
          name: 'Dr. Michael Chen',
          specialty: 'Neurologist',
          rating: 4.9,
          avatar: 'https://cdn.pixabay.com/photo/2018/09/12/12/14/man-3672010__340.jpg'
        },
        {
          id: 3,
          name: 'Dr. Emily Watson',
          specialty: 'Ophthalmologist',
          rating: 4.7,
          avatar: 'https://cdn.pixabay.com/photo/2018/01/15/07/51/woman-3083383__340.jpg'
        }
      ]
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
      this.loading = true
      try {
        const response = await getTelemedicineServices()
        const list = Array.isArray(response) ? response : []
        this.services = list.map(service => ({
          id: service.id,
          title: service.serviceName || service.name || 'Service',
          cost: `$${service.cost ?? 0}`,
          costValue: Number(service.cost) || 0,
          description: service.description || 'Telemedicine service',
          icon: this.mapServiceToIcon(service.serviceName || service.name || '')
        }))
      } catch (error) {
        console.error('Error fetching services:', error)
        // Fallback to default services if API fails
        this.services = [
          {
            id: 1,
            title: 'Eye Care',
            cost: '$200',
            description: 'Comprehensive eye examination and vision care services.',
            icon: 'EyeIcon'
          },
          {
            id: 2,
            title: 'Neurology',
            cost: '$200',
            description: 'Expert neurological consultation and treatment.',
            icon: 'BrainIcon'
          },
          {
            id: 3,
            title: 'Cardiac Care',
            cost: '$300',
            description: 'Heart health assessment and cardiovascular care.',
            icon: 'CardiacIcon'
          },
        ]
        if (this.$notification) {
          this.$notification.warning('Service Loading', 'Using default services. Please check your connection.')
        }
      } finally {
        this.loading = false
      }
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
  mounted() {
    this.fetchServices()
  }
}
</script>

<style scoped>
.appointment-container {
  @apply max-w-7xl mx-auto p-6 bg-gray-50 dark:bg-gray-900 min-h-screen;
}

.appointment-header {
  @apply text-center mb-8;
}

.appointment-title {
  @apply text-3xl font-bold text-gray-800 dark:text-gray-200 mb-2;
}

.appointment-subtitle {
  @apply text-gray-600 dark:text-gray-400;
}

.appointment-content {
  @apply space-y-8;
}

.section-title {
  @apply text-2xl font-semibold text-gray-800 dark:text-gray-200 mb-4;
}

.services-section {
  @apply mb-8;
}

.services-grid {
  @apply grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6;
}

.service-card {
  @apply bg-white dark:bg-gray-800 rounded-xl p-6 shadow-md border-2 border-transparent cursor-pointer transition-all duration-300 hover:shadow-lg hover:border-indigo-300 dark:hover:border-indigo-600;
}

.service-card.service-selected {
  @apply border-indigo-500 dark:border-indigo-400 bg-indigo-50 dark:bg-indigo-900/20 ring-2 ring-indigo-200 dark:ring-indigo-800;
}

.service-icon {
  @apply w-16 h-16 bg-indigo-100 dark:bg-indigo-900 rounded-lg flex items-center justify-center mb-4 text-indigo-600 dark:text-indigo-300;
}

.service-title {
  @apply text-xl font-bold text-gray-800 dark:text-gray-200 mb-2;
}

.service-description {
  @apply text-gray-600 dark:text-gray-400 mb-4 text-sm;
}

.service-footer {
  @apply flex items-center justify-between;
}

.service-cost {
  @apply text-2xl font-bold text-indigo-600 dark:text-indigo-400;
}

.service-badge {
  @apply px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded-lg font-semibold text-sm transition-colors;
}

.service-badge-selected {
  @apply bg-indigo-500 text-white;
}

.datetime-section {
  @apply bg-white dark:bg-gray-800 rounded-xl p-6 shadow-md;
}

.datetime-selector {
  @apply grid grid-cols-1 md:grid-cols-2 gap-6;
}

.selector-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.date-input,
.time-select {
  @apply w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:bg-gray-700 dark:text-gray-200;
}

.doctor-section {
  @apply bg-white dark:bg-gray-800 rounded-xl p-6 shadow-md;
}

.doctors-grid {
  @apply grid grid-cols-1 md:grid-cols-3 gap-4;
}

.doctor-card {
  @apply bg-gray-50 dark:bg-gray-700 rounded-lg p-4 cursor-pointer border-2 border-transparent transition-all hover:border-indigo-300 dark:hover:border-indigo-600;
}

.doctor-card.doctor-selected {
  @apply border-indigo-500 dark:border-indigo-400 bg-indigo-50 dark:bg-indigo-900/20;
}

.doctor-avatar {
  @apply w-20 h-20 rounded-full object-cover mx-auto mb-3;
}

.doctor-info {
  @apply text-center;
}

.doctor-name {
  @apply font-semibold text-gray-800 dark:text-gray-200 mb-1;
}

.doctor-specialty {
  @apply text-sm text-gray-600 dark:text-gray-400 mb-2;
}

.doctor-rating {
  @apply flex items-center justify-center gap-1;
}

.rating-stars {
  @apply text-yellow-400 text-sm;
}

.rating-value {
  @apply text-sm text-gray-600 dark:text-gray-400;
}

.appointment-actions {
  @apply mt-8 flex justify-center;
}

.continue-btn {
  @apply px-8 py-3 bg-gray-300 dark:bg-gray-700 text-gray-500 dark:text-gray-400 rounded-lg font-semibold transition-all disabled:cursor-not-allowed flex items-center;
}

.continue-btn-active {
  @apply bg-indigo-500 text-white hover:bg-indigo-600 shadow-lg transform hover:scale-105;
}
</style>
