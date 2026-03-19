<template>
  <div class="payment-container">
    <div class="payment-header">
      <h1 class="payment-title">{{ pendingAppointment ? 'Complete Payment' : 'Choose Your Plan' }}</h1>
      <p class="payment-subtitle">{{ pendingAppointment ? 'Pay for your selected telemedicine service' : 'Select the perfect plan for your healthcare needs' }}</p>
    </div>

    <div v-if="pendingAppointment" class="appointment-payment-section">
      <div class="appointment-summary-card">
        <h3>{{ pendingAppointment.service?.title || 'Telemedicine Service' }}</h3>
        <p class="service-cost">${{ pendingAppointment.service?.costValue ?? pendingAppointment.service?.cost?.replace?.(/[^0-9.]/g, '') ?? '0' }}</p>
        <button class="plan-button plan-button-selected" @click="showPaymentForm = true">
          Proceed to Payment
        </button>
      </div>
    </div>

    <div v-else class="payment-plans">
      <div 
        v-for="plan in plans" 
        :key="plan.id"
        :class="['plan-card', { 'plan-featured': plan.featured, 'plan-selected': selectedPlan === plan.id }]"
        @click="selectPlan(plan.id)"
      >
        <div v-if="plan.featured" class="plan-badge">Most Popular</div>
        <div class="plan-header">
          <h3 class="plan-name">{{ plan.name }}</h3>
          <div class="plan-price">
            <span class="plan-currency">$</span>
            <span class="plan-amount">{{ plan.price }}</span>
            <span class="plan-period">/ {{ plan.period }}</span>
          </div>
        </div>
        <ul class="plan-features">
          <li v-for="feature in plan.features" :key="feature" class="plan-feature">
            <svg class="feature-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
            </svg>
            <span>{{ feature }}</span>
          </li>
        </ul>
        <button 
          :class="['plan-button', { 'plan-button-selected': selectedPlan === plan.id }]"
          @click.stop="proceedToPayment(plan)"
        >
          {{ selectedPlan === plan.id ? 'Selected' : 'Select Plan' }}
        </button>
      </div>
    </div>

    <!-- Payment Form Modal -->
    <div v-if="showPaymentForm" class="payment-modal-overlay" @click="closePaymentForm">
      <div class="payment-modal" @click.stop>
        <div class="modal-header">
          <h2 class="modal-title">Payment Details</h2>
          <button class="modal-close" @click="closePaymentForm">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        
        <div class="modal-content">
          <div class="selected-plan-summary">
            <h3>{{ pendingAppointment ? (selectedPlanData?.name || 'Service') : (selectedPlanData?.name + ' Plan') }}</h3>
            <p class="summary-price">${{ selectedPlanData?.price }}{{ selectedPlanData?.period ? ' / ' + selectedPlanData.period : '' }}</p>
          </div>

          <form @submit.prevent="processPayment" class="payment-form">
            <div class="form-group">
              <label class="form-label">Card Number</label>
              <input 
                type="text" 
                v-model="paymentDetails.cardNumber"
                class="form-input"
                placeholder="1234 5678 9012 3456"
                maxlength="19"
                @input="formatCardNumber"
              />
            </div>

            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Expiry Date</label>
                <input 
                  type="text" 
                  v-model="paymentDetails.expiryDate"
                  class="form-input"
                  placeholder="MM/YY"
                  maxlength="5"
                  @input="formatExpiryDate"
                />
              </div>
              <div class="form-group">
                <label class="form-label">CVV</label>
                <input 
                  type="text" 
                  v-model="paymentDetails.cvv"
                  class="form-input"
                  placeholder="123"
                  maxlength="3"
                />
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">Cardholder Name</label>
              <input 
                type="text" 
                v-model="paymentDetails.cardholderName"
                class="form-input"
                placeholder="John Doe"
              />
            </div>

            <div class="form-group">
              <label class="form-label">Billing Address</label>
              <input 
                type="text" 
                v-model="paymentDetails.billingAddress"
                class="form-input"
                placeholder="123 Main Street"
              />
            </div>

            <div class="payment-summary">
              <div class="summary-row">
                <span>Subtotal</span>
                <span>${{ selectedPlanData?.price }}</span>
              </div>
              <div class="summary-row">
                <span>Tax</span>
                <span>${{ taxAmount }}</span>
              </div>
              <div class="summary-row summary-total">
                <span>Total</span>
                <span>${{ totalAmount }}</span>
              </div>
            </div>

            <button type="submit" class="submit-payment-btn" :disabled="!isFormValid || processing">
              <span v-if="processing">Processing...</span>
              <span v-else>Pay ${{ totalAmount }}</span>
            </button>
          </form>
        </div>
      </div>
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
      selectedPlan: null,
      showPaymentForm: false,
      processing: false,
      paymentDetails: {
        cardNumber: '',
        expiryDate: '',
        cvv: '',
        cardholderName: '',
        billingAddress: ''
      },
      plans: [
        {
          id: 1,
          name: 'Basic',
          price: '9.99',
          period: 'month',
          features: [
            '10GB Storage',
            'Basic Consultation',
            'Email Support',
            'Neurology Services',
            'Limited Access'
          ]
        },
        {
          id: 2,
          name: 'Pro',
          price: '24.99',
          period: 'month',
          featured: true,
          features: [
            'Unlimited Storage',
            'Priority Consultation',
            '24/7 Support',
            'Eye Care Services',
            'Neurology Services',
            'Cardiac Care',
            'Physiotherapy'
          ]
        },
        {
          id: 3,
          name: 'Premium',
          price: '49.99',
          period: 'month',
          features: [
            'Unlimited Storage',
            'VIP Consultation',
            '24/7 Premium Support',
            'All Medical Services',
            'Home Visits Available',
            'Unlimited Appointments',
            'Priority Scheduling'
          ]
        }
      ]
    }
  },
  mounted() {
    try {
      const stored = sessionStorage.getItem('pendingAppointment')
      if (stored) {
        this.pendingAppointment = JSON.parse(stored)
      }
    } catch (e) {
      console.warn('Could not parse pending appointment', e)
    }
  },
  computed: {
    selectedPlanData() {
      if (this.pendingAppointment?.service) {
        return {
          name: this.pendingAppointment.service.title || 'Service',
          price: this.pendingAppointment.service.costValue ?? this.pendingAppointment.service.cost?.replace?.(/[^0-9.]/g, '') ?? '0'
        }
      }
      return this.plans.find(p => p.id === this.selectedPlan)
    },
    taxAmount() {
      if (!this.selectedPlanData) return '0.00'
      const price = parseFloat(this.selectedPlanData.price)
      return this.pendingAppointment ? '0.00' : (price * 0.1).toFixed(2)
    },
    totalAmount() {
      if (!this.selectedPlanData) return '0.00'
      const price = parseFloat(this.selectedPlanData.price)
      return this.pendingAppointment ? price.toFixed(2) : (price + parseFloat(this.taxAmount)).toFixed(2)
    },
    isFormValid() {
      return this.paymentDetails.cardNumber.replace(/\s/g, '').length >= 16 &&
             this.paymentDetails.expiryDate.length === 5 &&
             this.paymentDetails.cvv.length >= 3 &&
             this.paymentDetails.cardholderName.trim() !== '' &&
             this.paymentDetails.billingAddress.trim() !== ''
    }
  },
  methods: {
    selectPlan(planId) {
      this.selectedPlan = planId
    },
    proceedToPayment(plan) {
      this.selectedPlan = plan.id
      this.showPaymentForm = true
    },
    closePaymentForm() {
      this.showPaymentForm = false
    },
    formatCardNumber(event) {
      let value = event.target.value.replace(/\s/g, '')
      let formattedValue = value.match(/.{1,4}/g)?.join(' ') || value
      this.paymentDetails.cardNumber = formattedValue
    },
    formatExpiryDate(event) {
      let value = event.target.value.replace(/\D/g, '')
      if (value.length >= 2) {
        value = value.substring(0, 2) + '/' + value.substring(2, 4)
      }
      this.paymentDetails.expiryDate = value
    },
    async processPayment() {
      this.processing = true
      try {
        if (this.pendingAppointment) {
          const { serviceId, service } = this.pendingAppointment
          const amountPaid = Number(service?.costValue ?? service?.cost?.replace?.(/[^0-9.]/g, '') ?? 0)
          if (!serviceId || amountPaid <= 0) {
            throw new Error('Invalid appointment data')
          }
          const paymentResult = await makePayment(serviceId, amountPaid)
          const paymentToken = paymentResult?.paymentToken ?? paymentResult
          if (!paymentToken) throw new Error('Payment failed')
          await scheduleAppointment(serviceId, paymentToken)
          sessionStorage.removeItem('pendingAppointment')
          if (this.$notification) {
            this.$notification.success('Success', 'Appointment scheduled successfully!')
          }
          this.$router.push('/dashboard')
        } else {
          await new Promise(r => setTimeout(r, 1500))
          if (this.$notification) {
            this.$notification.success('Payment Successful', 'Your subscription has been activated.')
          }
        }
        this.closePaymentForm()
      } catch (error) {
        if (this.$notification) {
          this.$notification.error('Payment Failed', error.message || 'Please try again.')
        }
      } finally {
        this.processing = false
      }
    }
  }
}
</script>

<style scoped>
.payment-container {
  @apply max-w-7xl mx-auto p-6 bg-gray-50 dark:bg-gray-900 min-h-screen;
}

.payment-header {
  @apply text-center mb-12;
}

.payment-title {
  @apply text-4xl font-bold text-gray-800 dark:text-gray-200 mb-4;
}

.payment-subtitle {
  @apply text-lg text-gray-600 dark:text-gray-400;
}

.appointment-payment-section {
  @apply max-w-md mx-auto mb-8;
}

.appointment-summary-card {
  @apply bg-white dark:bg-gray-800 rounded-xl shadow-lg p-8 text-center;
}

.appointment-summary-card h3 {
  @apply text-2xl font-bold text-gray-800 dark:text-gray-200 mb-2;
}

.appointment-summary-card .service-cost {
  @apply text-2xl font-bold text-indigo-600 dark:text-indigo-400 mb-6;
}

.payment-plans {
  @apply grid grid-cols-1 md:grid-cols-3 gap-8 mb-8;
}

.plan-card {
  @apply relative bg-white dark:bg-gray-800 rounded-xl shadow-lg p-8 transition-all duration-300 cursor-pointer border-2 border-transparent hover:border-indigo-300 dark:hover:border-indigo-600;
}

.plan-card.plan-featured {
  @apply border-indigo-500 dark:border-indigo-400 transform scale-105;
}

.plan-card.plan-selected {
  @apply border-indigo-500 dark:border-indigo-400 ring-4 ring-indigo-200 dark:ring-indigo-800;
}

.plan-badge {
  @apply absolute -top-4 left-1/2 transform -translate-x-1/2 bg-indigo-500 text-white px-4 py-1 rounded-full text-sm font-semibold;
}

.plan-header {
  @apply mb-6 pb-6 border-b border-gray-200 dark:border-gray-700;
}

.plan-name {
  @apply text-2xl font-bold text-gray-800 dark:text-gray-200 mb-4;
}

.plan-price {
  @apply flex items-baseline;
}

.plan-currency {
  @apply text-xl text-gray-600 dark:text-gray-400;
}

.plan-amount {
  @apply text-4xl font-bold text-gray-800 dark:text-gray-200;
}

.plan-period {
  @apply text-lg text-gray-600 dark:text-gray-400 ml-1;
}

.plan-features {
  @apply space-y-4 mb-8;
}

.plan-feature {
  @apply flex items-start text-gray-700 dark:text-gray-300;
}

.feature-icon {
  @apply w-5 h-5 text-green-500 mr-3 flex-shrink-0 mt-0.5;
}

.plan-button {
  @apply w-full py-3 px-6 rounded-lg font-semibold transition-colors bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200 hover:bg-gray-200 dark:hover:bg-gray-600;
}

.plan-button-selected {
  @apply bg-indigo-500 text-white hover:bg-indigo-600;
}

/* Payment Modal */
.payment-modal-overlay {
  @apply fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4;
}

.payment-modal {
  @apply bg-white dark:bg-gray-800 rounded-xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto;
}

.modal-header {
  @apply flex items-center justify-between p-6 border-b border-gray-200 dark:border-gray-700;
}

.modal-title {
  @apply text-2xl font-bold text-gray-800 dark:text-gray-200;
}

.modal-close {
  @apply p-2 rounded-lg text-gray-500 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors;
}

.modal-content {
  @apply p-6;
}

.selected-plan-summary {
  @apply bg-indigo-50 dark:bg-indigo-900/20 p-4 rounded-lg mb-6;
}

.selected-plan-summary h3 {
  @apply text-xl font-semibold text-gray-800 dark:text-gray-200 mb-1;
}

.summary-price {
  @apply text-lg text-indigo-600 dark:text-indigo-400 font-semibold;
}

.payment-form {
  @apply space-y-4;
}

.form-group {
  @apply mb-4;
}

.form-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.form-input {
  @apply w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:bg-gray-700 dark:text-gray-200;
}

.form-row {
  @apply grid grid-cols-2 gap-4;
}

.payment-summary {
  @apply bg-gray-50 dark:bg-gray-700 p-4 rounded-lg space-y-2 mb-6;
}

.summary-row {
  @apply flex justify-between text-gray-700 dark:text-gray-300;
}

.summary-total {
  @apply text-xl font-bold text-gray-800 dark:text-gray-200 pt-2 border-t border-gray-300 dark:border-gray-600;
}

.submit-payment-btn {
  @apply w-full py-3 px-6 bg-indigo-500 text-white rounded-lg font-semibold hover:bg-indigo-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed;
}

@media (max-width: 768px) {
  .payment-plans {
    @apply grid-cols-1;
  }
  
  .plan-card.plan-featured {
    @apply transform scale-100;
  }
}
</style>
