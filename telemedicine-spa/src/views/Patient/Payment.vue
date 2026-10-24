<template>
  <div class="max-w-3xl mx-auto p-6">
    <div class="text-center mb-8">
      <h1 class="font-display text-3xl font-bold text-slate-800 dark:text-slate-100">Appointment payment</h1>
      <p class="text-slate-600 dark:text-slate-400 mt-2">Sandbox checkout for your telemedicine appointment</p>
    </div>

    <div v-if="paymentComplete" class="card-modern p-8 text-center">
      <div class="w-14 h-14 rounded-full bg-green-100 text-green-700 text-3xl flex items-center justify-center mx-auto mb-4">✓</div>
      <h2 data-testid="payment-success" class="text-2xl font-bold text-slate-800 dark:text-slate-100">Appointment confirmed</h2>
      <p class="text-slate-600 dark:text-slate-400 mt-2 mb-6">
        The simulated payment was approved and the appointment was created. No real money was charged.
      </p>
      <dl class="text-left divide-y divide-slate-100 dark:divide-slate-700 mb-6">
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Service</dt><dd class="font-semibold">{{ booking.serviceName }}</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Amount</dt><dd class="font-semibold">${{ Number(booking.amount).toFixed(2) }}</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Appointment</dt><dd class="font-semibold">#{{ booking.appointmentId }}</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">When</dt><dd class="font-semibold text-right">{{ formatDate(booking.startUtc) }}</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Physician</dt><dd class="font-semibold">{{ booking.physicianName }}</dd></div>
      </dl>
      <div class="flex flex-col sm:flex-row gap-3 justify-center">
        <router-link :to="`/dashboard/appointments/${booking.appointmentId}`" class="btn-primary text-center">View appointment</router-link>
        <router-link to="/dashboard" class="btn-secondary text-center">Back to dashboard</router-link>
      </div>
    </div>

    <div v-else-if="pendingAppointment" class="card-modern p-8">
      <h2 class="text-2xl font-bold text-slate-800 dark:text-slate-100">{{ pendingAppointment.serviceName }}</h2>
      <p class="text-slate-600 dark:text-slate-400 mt-1 mb-6">{{ pendingAppointment.serviceDescription }}</p>

      <dl class="divide-y divide-slate-100 dark:divide-slate-700 mb-6">
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">When</dt><dd class="font-semibold text-right">{{ formatDate(pendingAppointment.startUtc) }}</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Duration</dt><dd class="font-semibold">{{ pendingAppointment.durationMinutes }} minutes</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Physician</dt><dd class="font-semibold">{{ pendingAppointment.physicianName }}</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Speciality</dt><dd class="font-semibold">{{ pendingAppointment.physicianSpecialization }}</dd></div>
        <div class="flex justify-between gap-4 py-3 text-sm"><dt class="text-slate-500">Amount due</dt><dd class="text-xl font-bold text-primary-600 dark:text-primary-400">${{ amount.toFixed(2) }}</dd></div>
      </dl>

      <div class="rounded-xl border border-amber-200 bg-amber-50 p-4 text-sm text-amber-900 dark:border-amber-800 dark:bg-amber-900/20 dark:text-amber-200 mb-6">
        <strong>Sandbox / simulated payment.</strong>
        No card number, CVV or real payment details are collected. The API records a synthetic payment
        and returns a one-time token used to create this appointment.
      </div>

      <p v-if="error" class="rounded-xl border border-red-200 bg-red-50 p-3 text-sm text-red-700 dark:border-red-800 dark:bg-red-900/20 dark:text-red-300 mb-4">
        {{ error }}
      </p>

      <div class="flex flex-col sm:flex-row gap-3">
        <button type="button" data-testid="pay-button" class="btn-primary" :disabled="processing || amount <= 0" @click="processPayment">
          {{ processing ? 'Processing simulated payment...' : `Pay $${amount.toFixed(2)} (demo)` }}
        </button>
        <button type="button" class="btn-secondary" :disabled="processing" @click="cancelCheckout">Cancel</button>
      </div>
    </div>

    <div v-else class="card-modern p-8 text-center">
      <h2 class="text-2xl font-bold text-slate-800 dark:text-slate-100">No appointment selected</h2>
      <p class="text-slate-600 dark:text-slate-400 mt-2 mb-6">Choose a service, physician and available slot before continuing to payment.</p>
      <router-link to="/dashboard/book-service" class="btn-primary inline-flex justify-center">Book an appointment</router-link>
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
        startUtc: null,
        physicianName: null,
      },
    }
  },
  computed: {
    amount() {
      return Number(this.pendingAppointment?.costValue ?? 0)
    },
  },
  mounted() {
    try {
      const stored = sessionStorage.getItem('pendingAppointment')
      if (stored) this.pendingAppointment = JSON.parse(stored)
    } catch (e) {
      console.warn('Could not parse pending appointment', e)
    }
  },
  methods: {
    formatDate(value) {
      if (!value) return 'Not selected'
      const date = new Date(value)
      if (Number.isNaN(date.getTime())) return value
      return date.toLocaleString(undefined, {
        weekday: 'short', year: 'numeric', month: 'short', day: 'numeric',
        hour: '2-digit', minute: '2-digit',
      })
    },
    async processPayment() {
      if (this.processing || !this.pendingAppointment) return
      this.processing = true
      this.error = ''
      try {
        const serviceId = Number(this.pendingAppointment.serviceId)
        if (!serviceId) throw new Error('The selected service is missing.')
        if (this.amount <= 0) throw new Error('The selected service has no payable amount.')
        if (!this.pendingAppointment.startUtc) throw new Error('The selected slot is missing.')

        const payment = await makePayment(serviceId, this.amount)
        const paymentToken = payment?.paymentToken
        if (!paymentToken) throw new Error('The payment service did not return a token.')

        const created = await scheduleAppointment(
          serviceId,
          paymentToken,
          this.pendingAppointment.startUtc,
          Number(this.pendingAppointment.physicianId),
          Number(this.pendingAppointment.durationMinutes) || 30,
        )

        this.booking = {
          appointmentId: created?.id ?? created,
          serviceName: this.pendingAppointment.serviceName,
          amount: this.amount,
          startUtc: this.pendingAppointment.startUtc,
          physicianName: this.pendingAppointment.physicianName,
        }
        sessionStorage.removeItem('pendingAppointment')
        this.paymentComplete = true
        if (this.$notification) this.$notification.success('Appointment booked', 'Simulated payment approved.')
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
    },
  },
}
</script>
