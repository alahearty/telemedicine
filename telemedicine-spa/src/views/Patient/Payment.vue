<template>
  <div class="payment-container">
    <div class="payment-header">
      <h1 class="payment-title">Appointment Payment</h1>
      <p class="payment-subtitle">Sandbox checkout for your telemedicine appointment</p>
    </div>

    <div v-if="paymentComplete" class="success-card">
      <div class="success-icon">✓</div>
      <h2>Appointment confirmed</h2>
      <p class="success-note">
        The simulated payment was approved and the appointment was created. No real money was charged.
      </p>
      <dl class="summary-list">
        <div><dt>Service</dt><dd>{{ booking.serviceName }}</dd></div>
        <div><dt>Amount</dt><dd>${{ booking.amount.toFixed(2) }}</dd></div>
        <div><dt>Appointment</dt><dd>#{{ booking.appointmentId }}</dd></div>
        <div v-if="booking.appointmentDate"><dt>Requested date</dt><dd>{{ formatDate(booking.appointmentDate) }}</dd></div>
        <div v-if="booking.physicianName"><dt>Physician</dt><dd>{{ booking.physicianName }}</dd></div>
      </dl>
      <router-link to="/dashboard" class="submit-payment-btn inline-flex justify-center">Back to dashboard</router-link>
    </div>

    <div v-else-if="pendingAppointment" class="checkout-card">
      <h2>{{ pendingAppointment.service?.title || 'Telemedicine service' }}</h2>
      <p class="service-description">{{ pendingAppointment.service?.description || 'Selected telemedicine consultation' }}</p>

      <dl class="summary-list">
        <div><dt>Requested date</dt><dd>{{ requestedDateLabel }}</dd></div>
        <div><dt>Requested time</dt><dd>{{ pendingAppointment.selectedTime || 'Not selected' }}</dd></div>
        <div><dt>Physician</dt><dd>{{ selectedPhysicianLabel }}</dd></div>
        <div class="amount-row"><dt>Amount due</dt><dd>${{ amount.toFixed(2) }}</dd></div>
      </dl>

      <div class="sandbox-note">
        <strong>Sandbox / simulated payment.</strong>
        No card number, CVV or real payment details are collected. The API records a synthetic payment
        and returns a one-time token used to create the appointment.
      </div>

      <p v-if="error" class="error-box">{{ error }}</p>

      <div class="actions">
        <button type="button" class="submit-payment-btn" :disabled="processing || amount <= 0" @click="processPayment">
          {{ processing ? 'Processing simulated payment...' : `Pay $${amount.toFixed(2)} (demo)` }}
        </button>
        <button type="button" class="secondary-btn" :disabled="processing" @click="cancelCheckout">
          Cancel
        </button>
      </div>
    </div>

    <div v-else class="empty-card">
      <h2>No appointment selected</h2>
      <p>Choose a service and a time before continuing to payment.</p>
      <router-link to="/dashboard/book-service" class="submit-payment-btn inline-flex justify-center">
        Book an appointment
      </router-link>
    </div>
  </div>
</template>

<script>
import { makePayment, scheduleAppointment } from '@/api'

export default {
  name: 'Payment',
  data() {
    return {
      pendingAppointment: null,
      processing: false,
      paymentComplete: false,
      error: '',
      booking: {
        appointmentId: null,
        serviceName: '',
        amount: 0,
        appointmentDate: null,
        physicianName: null
      }
    }
  },
  mounted() {
    try {
      const stored = sessionStorage.getItem('pendingAppointment')
      if (stored) this.pendingAppointment = JSON.parse(stored)
    } catch (e) {
      console.warn('Could not parse pending appointment', e)
    }
  },
  computed: {
    amount() {
      const service = this.pendingAppointment?.service
      return Number(service?.costValue ?? 0)
    },
    requestedDateLabel() {
      return this.pendingAppointment?.selectedDate
        ? this.formatDate(this.pendingAppointment.selectedDate)
        : 'Not selected'
    },
    selectedPhysicianLabel() {
      const doctor = this.pendingAppointment?.selectedDoctor
      if (!doctor) return 'Any available physician'
      return `Physician #${doctor}`
    }
  },
  methods: {
    formatDate(value) {
      if (!value) return 'Not selected'
      const date = new Date(value)
      if (Number.isNaN(date.getTime())) return value
      return date.toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' })
    },
    buildAppointmentDate() {
      const dateValue = this.pendingAppointment?.selectedDate
      const timeValue = this.pendingAppointment?.selectedTime
      if (!dateValue || !timeValue) return null
      const match = /^(\d{1,2}):(\d{2})\s*(AM|PM)$/i.exec(timeValue.trim())
      if (!match) return null
      let hours = Number(match[1])
      const minutes = Number(match[2])
      const meridiem = match[3].toUpperCase()
      if (meridiem === 'PM' && hours < 12) hours += 12
      if (meridiem === 'AM' && hours === 12) hours = 0
      const date = new Date(`${dateValue}T00:00:00`)
      date.setHours(hours, minutes, 0, 0)
      return date.toISOString()
    },
    async processPayment() {
      if (this.processing || !this.pendingAppointment) return
      this.processing = true
      this.error = ''
      try {
        const serviceId = Number(this.pendingAppointment.serviceId)
        if (!serviceId) throw new Error('The selected service is missing.')
        if (this.amount <= 0) throw new Error('The selected service has no payable amount.')

        const payment = await makePayment(serviceId, this.amount)
        const paymentToken = payment?.paymentToken
        if (!paymentToken) throw new Error('The payment service did not return a token.')

        const appointmentDate = this.buildAppointmentDate()
        const physicianId = this.pendingAppointment.selectedDoctor || null
        const appointmentId = await scheduleAppointment(serviceId, paymentToken, appointmentDate, physicianId)

        this.booking = {
          appointmentId: appointmentId ?? 'created',
          serviceName: this.pendingAppointment.service?.title || 'Telemedicine service',
          amount: this.amount,
          appointmentDate,
          physicianName: this.pendingAppointment.selectedDoctor
            ? `Physician #${this.pendingAppointment.selectedDoctor}`
            : null
        }
        sessionStorage.removeItem('pendingAppointment')
        this.paymentComplete = true
        if (this.$notification) {
          this.$notification.success('Appointment booked', 'Simulated payment approved.')
        }
      } catch (err) {
        this.error = err.message || 'The simulated payment could not be completed.'
        if (this.$notification) this.$notification.error('Payment failed', this.error)
      } finally {
        this.processing = false
      }
    },
    cancelCheckout() {
      sessionStorage.removeItem('pendingAppointment')
      this.$router.push('/dashboard/book-service')
    }
  }
}
</script>

<style scoped>
.payment-container {
  @apply max-w-3xl mx-auto p-6 min-h-screen;
}

.payment-header {
  @apply text-center mb-8;
}

.payment-title {
  @apply font-display text-3xl font-bold text-slate-800 dark:text-slate-100 mb-2;
}

.payment-subtitle {
  @apply text-slate-600 dark:text-slate-400;
}

.checkout-card,
.success-card,
.empty-card {
  @apply bg-white dark:bg-slate-800 rounded-2xl shadow-soft border border-slate-100 dark:border-slate-700 p-8;
}

.checkout-card h2,
.success-card h2,
.empty-card h2 {
  @apply text-2xl font-bold text-slate-800 dark:text-slate-100 mb-2;
}

.service-description {
  @apply text-slate-600 dark:text-slate-400 mb-6;
}

.summary-list {
  @apply divide-y divide-slate-100 dark:divide-slate-700 mb-6;
}

.summary-list > div {
  @apply flex justify-between gap-4 py-3 text-sm;
}

.summary-list dt {
  @apply text-slate-500 dark:text-slate-400;
}

.summary-list dd {
  @apply font-semibold text-slate-800 dark:text-slate-100 text-right;
}

.amount-row dd {
  @apply text-xl text-primary-600 dark:text-primary-400;
}

.sandbox-note {
  @apply bg-amber-50 dark:bg-amber-900/20 border border-amber-200 dark:border-amber-800 text-amber-900 dark:text-amber-200 rounded-xl p-4 text-sm leading-relaxed mb-6;
}

.error-box {
  @apply bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 text-red-700 dark:text-red-300 rounded-xl p-3 text-sm mb-4;
}

.actions {
  @apply flex flex-col sm:flex-row gap-3;
}

.submit-payment-btn {
  @apply w-full py-3 px-6 bg-primary-500 text-white rounded-xl font-semibold hover:bg-primary-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-center;
}

.secondary-btn {
  @apply w-full sm:w-auto py-3 px-6 rounded-xl font-semibold border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-200 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors disabled:opacity-50;
}

.success-card {
  @apply text-center;
}

.success-icon {
  @apply w-14 h-14 rounded-full bg-green-100 text-green-700 text-3xl flex items-center justify-center mx-auto mb-4;
}

.success-note {
  @apply text-slate-600 dark:text-slate-400 mb-6;
}

.success-card .summary-list {
  @apply text-left;
}

.empty-card {
  @apply text-center;
}
</style>
